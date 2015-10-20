App.Views.CreateTourView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('tours/tourNew'),

    events: {
        'click #createTourBtn': function (e) {
            e.preventDefault();
            this.createTourClicked();
        },
    },

    bindings: {
        "#startCity": "value: start().getName()",
        "#destCity": "value: dest().getName()",
        "#expirationDate": "value:expirationDate, events:['onChange']",
        "#freeSlots": "value:freeSlots, events:['onChange']",
        "#payment": "value:payment, events:['onChange']"
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        Backbone.Validation.bind(this);
        var model = this.model;
        this.listenTo(this.model, "change", function () {
            model.isValid(['expirationDate', 'startCity', 'destCity', 'freeSlots'])
        });
        this.mapView = new App.Views.MapView({ model: this.model });
        this.mapView.initializeMapCenter();

        this.tourProfileView = new App.Views.TourProfileView({ model: this.model.getProfile() });
        
        this.childViews = [];
        this.childViews.push(this.tourProfileView, this.mapView);
    },
    stopValidation: function () {
        Backbone.Validation.unbind(this);
    },
    render: function () {
        this.$el.html(this.template);
        this.applyBindings();

        var self = this;

        this.$('#startCity').geocomplete().bind("geocode:result", function (event, result) {
            self.updateStartCity(result);
        });

        this.$('#destCity').geocomplete().bind("geocode:result", function (event, result) {
            self.updateDestCity(result);
        });

        this.$('#expirationDate').datepicker({
            format: 'dd/mm/yyyy',
            startDate: 'd',
            language: "ru",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true
        });

        this.mapView.el = 'mapCanvas';
        this.mapView.render();

        this.tourProfileView.$el = this.$('#tourProfile');
        this.tourProfileView.render();

        this.waypointsTags = this.$('#tags');
        var waypointsInput = this.$('#waypoints');

        this.waypointsTags.tagsinput();
        this.$('.bootstrap-tagsinput').children('input').attr('style', 'display: none');
        this.fillTags(this.waypointsTags);

        waypointsInput.geocomplete().bind("geocode:result", function (event, result) {
            self.updateWaypoints(result);
        });

        this.waypointsTags.on('itemAdded', function (event) {
            // event.item: contains the item
            // event.cancel: set to true to prevent the item getting added
            waypointsInput.val('');
        });

        this.waypointsTags.on('beforeItemRemove', function (event) {
            // event.item: contains the item
            var toRemove = self.model.wayPoints().where({ cityNameRu: event.item })[0];
            self.model.wayPoints().remove(toRemove);
            self.model.wayPoints().trigger('newWaypoint');
        });

        $('#paymentTooltip').tooltip({
            placement: "bottom",
            trigger: "focus",
            title: "Если не указать, то оплата будет считаться договорной, ограничение 5 тысяч рублей"
        });

        return this;
    },

    updateStartCity: function (result) {

        var latitude = result.geometry.location.A;
        var longtitude = result.geometry.location.F;
        var region = result.address_components[2].long_name;

        if (this.model.dest().getTitle() == region + ', ' + result.name) {
            App.Alerts.showAlert("DestExist", "warning");
            this.$('#startCity').val(this.model.start().getName());
            return;
        }
        if (this.model.isWpntExistByLtLng(latitude, longtitude)) {
            App.Alerts.showAlert("StartIsWaypoint", "warning");
            this.$('#startCity').val(this.model.start().getName());
            return;
        }

        this.model.parseStartCity(latitude, longtitude, result.name, region);
    },

    updateDestCity: function (result) {

        var latitude = result.geometry.location.A;
        var longtitude = result.geometry.location.F;
        var region = result.address_components[2].long_name;

        if (this.model.start().getTitle() == region + ', ' + result.name) {
            App.Alerts.showAlert("StartExist", "warning");
            this.$('#destCity').val(this.model.dest().getName());
            return;
        }
        if (this.model.isWpntExistByLtLng(latitude, longtitude)) {
            App.Alerts.showAlert("DestIsWaypoint", "warning");
            this.$('#destCity').val(this.model.dest().getName());
            return;
        }

        this.model.parseDestCity(latitude, longtitude, result.name, region);
    },

    updateWaypoints: function (result) {
        var latitude = result.geometry.location.A;
        var longtitude = result.geometry.location.F;
        var region = result.address_components[2].long_name;

        if (this.model.start().isEmpty() || this.model.dest().isEmpty()) {
            App.Alerts.showAlert("StartDestRequired", "warning");
            this.$('#waypoints').val('');
            return;
        }

        var waypoint = new App.Models.CityModel({
            latitude: latitude,
            longtitude: longtitude,
            cityNameRu: result.name,
            regionNameRu: region
        });

        if (this.model.start().isCityExist(waypoint) || this.model.dest().isCityExist(waypoint) || this.model.isWayPointExist(waypoint)) {
            App.Alerts.showAlert("PointExist", "warning");
            this.$('#waypoints').val('');
            return;
        }

        this.waypointsTags.tagsinput('add', result.name);
        this.model.wayPoints().add(waypoint);
        this.model.wayPoints().trigger('newWaypoint');

        if (App.debug) console.log('Waypoint added :', waypoint.toJSON());
    },


    fillTags: function (el) {
        this.model.wayPoints().each(function (waypoint) { el.tagsinput('add', waypoint.getName()); });
    },

    createTourClicked: function () {
        if (this.model.isValid(['startCity', 'destCity', 'freeSlots', 'expirationDate', 'payment'])) {
            if (App.debug) console.log('New tour is: ', this.model);
            this.$('#createTourBtn').button('loading');
            if (App.session.status()) {
                var self = this;

                var mapRoute = this.model.wayPoints().clone();
                mapRoute.unshift(this.model.start().clone());
                mapRoute.push(this.model.dest().clone());
                this.model.set('wayPoints', mapRoute);

                this.model.save(null, {
                    success: function (model, response) {
                        App.Alerts.showAlert(response.message, "success", true);
                        App.router.navigate('/tours', { trigger: true });
                        //self.$('#createTourBtn').button('reset');
                    },
                    customError: true,
                    error: function (model, response) {
                        self.$('#createTourBtn').button('reset');
                        App.Alerts.showAlert(JSON.parse(response.responseText).message, "error", true);
                    }
                });
            } else {
                App.Views.header.onLoginClick();
                this.$('#createTourBtn').button('reset');
                App.Alerts.showAlert("NotAuthorized", "warning");
            }
        }
    },
});