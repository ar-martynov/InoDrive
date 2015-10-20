App.Views.SearchToursResultItemView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('tours/tourItemSearchResult'),
    initialize: function() {
        this.$el.append(this.template);
        if (this.model.get('freeSlots') == 0) this.$("#freeSlots").attr('class', 'text-danger');
        return this;
    },
    bindings: {
        "#ownerName": "text:owner",
        "#startName": "text:startCity",
        "#destName": "text:destCity",
        "#freeSlots": "text:freeSlots",
        "#expirationDate" : "text: expirationDate",
        "#payment": "text:payment",
        "a[class='list-group-item']": "attr:{\'href\': getTourUrl()}",
    },
});


App.Views.UserToursItemView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('tours/tourCollectionItem'),
    initialize: function () {
        
        this.$el.append(this.template);
        if (this.model.get('freeSlots') == 0) this.$("#freeSlots").attr('class', 'text-danger');
        if (this.model.isExpired()) {
            this.$("#tourStatus").text('Поездка завершена.');
            this.$(".link-modify").attr('disabled', true);
        }
        
        return this;
    },
    events: {
        "click a[class='btn btn-default btn-xs link-delete'] ": "deleteEventHandler"
    },
    bindings: {
        "div[class='panel panel-default']": "attr:{\'id\': id}",
        "#ownerName": "text:owner",
        "#startName": "text:startCity",
        "#destName": "text:destCity",
        "#freeSlots": "text:freeSlots",
        "#expirationDate": "text: expirationDate",
        "#payment": "text:payment",
        "a[class='list-group-item link-details']": "attr:{\'href\': getTourUrl()}",
        "a[class='btn btn-default btn-xs link-modify']": "attr:{\'href\': getModifyTourUrl()}",
        "a[class='btn btn-default btn-xs link-delete']": "attr:{\'id\': id}",
        
    },
    deleteEventHandler: function (e) {
        var model = this.model;
        model.destroy({
            success: function (model, response) {
                        App.Alerts.showAlert(response.message, "success", true);
            },
        });
    }
});


App.Views.TourView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('tours/tourView'),
    events: {
        'click #bid' : 'betHandler'
    },
    bindings: {
        "#startName": " text: start()",
        "#destName": " text: dest()",
        "#freeSlots": " text: freeSlots()",
        "#payment": "text: payment()",
        "#expirationDate": "text: expirationDate()",
        "#userCar": "text: carDescription()",
        "#carImageLink": "attr:{href:carImage()}",
        "#carImage": "attr:{src:carImage()}",
        "#additionalInfo" : "text: additionalInfo()"
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this))); 
        this.userView = new App.Views.UserProfileView({ model: this.model.getUserProfile() });
        this.mapView = new App.Views.MapView({ model: this.model });
        this.listenTo(this.model, 'change', this.userView.render());
        this.childViews = [];
        this.childViews.push(this.userView, this.mapView);
        return this;
    },

    render: function () {
        this.$el.html(this.template);
        this.applyBindings();
        if (this.model.isOwner()) this.$('#bid').remove();
        if (this.model.isExpired()) {
            this.$('#bid').attr({ 'disabled': true, 'id': '' }).html('Поездка завершена');
            this.$('#expirationDate').addClass('text-danger');
            if (App.session.status() && !this.model.isOwner()) {
                this.$('#ratingBar').raty({
                    starOff: '../Content/images/star-off-big.png',
                    starOn: '../Content/images/star-on-big.png',
                    click: function () {
                        //TODO: rating handler!
                        App.Alerts.showAlert('Rated', 'success')
                    }
                });
            }
        } else this.$('#ratingWrap').remove();

        if (this.model.alreadyBet()) this.$('#bid').attr({ 'disabled': true, 'id': '' }).html('Заявка отправлена');
        if (this.model.get('freeSlots') == 0) this.$("#freeSlots").attr('class', 'text-danger');
        this.userView.$el = this.$("#userInfo");
        this.mapView.el = 'mapCanvas';
        this.userView.render();
        this.mapView.render();
        var wpnts = this.model.wayPoints();
        for (var i = 0; i < this.model.wayPoints().length; i++) {
            this.$('#wpnts').append(String.format('<span class="label label-primary">{0}</span> &nbsp', this.model.wayPoints().at(i).get('cityNameRu')));
        }
        if (this.model.wayPoints().length == 0) this.$('#wpnts').append("отсутствуют");
        if (!this.model.isCarImageExist()) this.$('#carImageLink').remove();
        return this;
    },
    betHandler: function () {
        if (App.session.status()) {
            var model = new App.Models.NewBidModel({ id: this.model.get('id') });
            var self = this;
            Backbone.sync('get', model, {
                url: String.format("{0}/bids/addBid?id={1}", App.apiPath, model.get('id')),
                success: function (response) {
                    self.$('#bid').attr({ 'disabled': true, 'id': '' }).html('Заявка отправлена');
                    App.Alerts.showAlert(response.message, 'success', true);
                    App.router.navigate('/bids/mybids', { trigger: true });
                }
            });
        } else {
            App.Alerts.showAlert("NotAuthorized", "error");
            App.Views.header.onLoginClick();
        }
    },
});

App.Views.UserToursView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('tours/myTours'),
    bindings: {
        "#searchResult": "collection: $collection, itemView: \'itemView\'"
    },
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));

        this.collection = this.model.getTours();
        this.itemView = App.Views.UserToursItemView;

        this.childViews = [];
        this.childViews.push(this.tourView, this.advancedSearchView);
        
    },
    render: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        this.$el.html(this.template);   
        this.applyBindings();
        if (this.model.getTours().length == 0) this.$("#searchResult").html(App.Templates.get('system/noAds'))
        var self = this;
        this.$('#paginator').bootpag({
            total: this.model.getTotalPages() == 1 ? 0 : this.model.getTotalPages(),
            page: this.model.getCurrentPage(),
            maxVisible: 8
        }).on("page", function (event, num) {
            self.model.setPage(num);
            self.model.fetch();
        });
        return this;
    },
    
});