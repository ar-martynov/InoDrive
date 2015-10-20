App.Views.SearchTourView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('forms/indexSearch'),

    events: {
        'click #indexSearchButton': function (e) {
            e.preventDefault();
            this.indexSubmitClicked();
        },
        'click #searchButton': function (e) {
            e.preventDefault();
            this.submitClicked();
        },
        'change #chkbx-advancedSearch': 'advancedSearchClickEventHandler',
    },

    bindings: {
        "#startCity": "value: start().getName()",
        "#destCity": "value: dest().getName()",
        "#expirationDate": "value:expirationDate, events:['onChange']",
        "#freeSlots": "value:freeSlots, events:['onChange']",
        "#searchResult": "collection: $collection, itemView: \'itemView\'"
    },

    initialize: function (options) {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        Backbone.Validation.bind(this);
        var model = this.model;
        this.listenTo(this.model, "change", function () {
            model.isValid(['expirationDate', 'startCity', 'destCity', 'freeSlots'])
        });

        if (this.model.searchResult == null) this.model.searchResult = new App.Collections.SearchToursResult();
        this.collection = this.model.searchResult;
        this.itemView = App.Views.SearchToursResultItemView;

        this.model = options.model;
        this.newSearch = options.newSearch;

        this.advancedSearchView = new App.Views.AdvancedSearchTourProfileView({ model: this.model.getAdvancedSearchProfile() });
        this.advancedSearchFlag = false;

        this.childViews = [];
        this.childViews.push(this.tourView, this.advancedSearchView);   
    },

    stopValidation: function () {
        Backbone.Validation.unbind(this);
    },

    render: function () {

        var self = this;
        var selfModel = this.model;

        if (this.newSearch) {
            this.$el.html(this.template);
        }
        else {
            this.$el.html(App.Templates.get('tours/SearchTours'));
            this.$('#searchForm').html(App.Templates.get('forms/horizontalSearch'));

            this.$('#paginator').bootpag({
                total: this.model.getTotalPages() == 1 ? 0 : this.model.getTotalPages(),
                page: this.model.getCurrentPage(),
                maxVisible: 8
            }).on("page", function (event, num) {
                self.model.set('page', num);
                self.search();
            });
        }
        this.applyBindings();

        this.$('#startCity').geocomplete().bind("geocode:result", function (event, result) {
            var latitude = result.geometry.location.A;
            var longtitude = result.geometry.location.F;
            var region = result.address_components[2].long_name;

            self.model.start().setCity(latitude, longtitude, result.name, region);

            if (App.debug) console.log('StartCity:', self.model.start().toJSON());
        });

        this.$('#destCity').geocomplete().bind("geocode:result", function (event, result) {
            var latitude = result.geometry.location.A;
            var longtitude = result.geometry.location.F;
            var region = result.address_components[2].long_name;

            self.model.dest().setCity(latitude, longtitude, result.name, region);
            if (App.debug) console.log('DestCity: ', self.model.dest().toJSON());
        });

        this.$('#expirationDate').datepicker({
            format: 'dd/mm/yyyy',
            startDate: 'd',
            language: "ru",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true
        });
        this.$('#freeSlots').tooltip({
            placement: "top",
            trigger: "focus",
            title: "Максимальное количество пассажиров 8."
        });
       
        this.advancedSearchView.$el = this.$("#collapseContainer");
        this.advancedSearchView.render();


        var waypointsTags = this.$('#tags');
        var waypointsInput = this.$('#waypoints');

        waypointsTags.tagsinput();
        this.$('.bootstrap-tagsinput').children('input').attr('style', 'display: none');
        this.fillTags(waypointsTags);

        waypointsInput.geocomplete().bind("geocode:result", function (event, result) {
            if (selfModel.start().isEmpty() || selfModel.dest().isEmpty()) {
                App.Alerts.showAlert("StartDestRequired", "warning");
                self.$('#waypoints').val('');
                return;
            }

            var waypoint = new App.Models.CityModel({
                latitude: result.geometry.location.A,
                longtitude: result.geometry.location.F,
                cityNameRu: result.name,
                regionNameRu: result.address_components[2].long_name
            });

            if (selfModel.start().isCityExist(waypoint) || selfModel.dest().isCityExist(waypoint) || selfModel.isWayPointExist(waypoint)) {
                App.Alerts.showAlert("PointExist", "warning");
                self.$('#waypoints').val('');
                return;
            }

            waypointsTags.tagsinput('add', result.name);
            selfModel.wayPoints().add(waypoint);
            selfModel.wayPoints().trigger('newWaypoint');

            if (App.debug) console.log('Waypoint added :', waypoint.toJSON());
        });

        waypointsTags.on('itemAdded', function (event) {
            // event.item: contains the item
            // event.cancel: set to true to prevent the item getting added
            waypointsInput.val('');
        });

        waypointsTags.on('beforeItemRemove', function (event) {
            // event.item: contains the item
            var toRemove = selfModel.wayPoints().where({ cityNameRu: event.item })[0];
            selfModel.wayPoints().remove(toRemove);
            selfModel.wayPoints().trigger('newWaypoint');
        });
        
        return this;
    },

    advancedSearchClickEventHandler: function (e) {
        this.advancedSearchFlag = !this.advancedSearchFlag;
        this.$('#advancedSearchCollapse').collapse('toggle');
    },

    fillTags: function (el) {
        this.model.wayPoints().each(function (waypoint) { el.tagsinput('add', waypoint.getName()); });
    },

    indexSubmitClicked: function () {
        if (this.search()) {
            this.newSearch = false;
            this.model.set('page', 1);
            this.model.set('totalPages', 0);

            App.router.navigate('/tours/search', { trigger: true });
        }
    },

    submitClicked: function () {
        this.model.resetPaging();
        this.search();
    },

    search: function () {
        if (this.model.isValid(['expirationDate', 'startCity', 'destCity', 'freeSlots'])) {
            this.model.swapUrl(this.advancedSearchFlag);
            var self = this;

            this.$('#searchButton').button('loading');

            this.model.save(null, {
                success: function (model, response) {
                    if (App.debug) console.log("Search result: ", response);
                    self.$('#searchButton').button('reset');
                    if (response.success == false || response.tours.length == 0) {
                        self.$('#searchResult').html(App.Templates.get('system/noResults'));
                        self.$('#paginator').empty();
                        return;
                    }
                    self.collection.reset();
                    _.each(response.tours, function (tour) {
                        self.collection.add(new App.Models.ShortTourSearchResultItemModel(tour));
                    });
                    if (response.totalPages < 2) self.$('#paginator').empty();
                    else {
                        self.model.set('totalPages', response.totalPages);
                        self.$('#paginator').bootpag({ total: response.totalPages, page: (response.page) });
                    }
                }
            });
            return true;
        }
        return false;
    },
});