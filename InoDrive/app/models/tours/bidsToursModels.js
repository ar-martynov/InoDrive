App.Models.BidsToursModel = Backbone.Epoxy.Model.extend({
    defautlts: {
        bids: null,
        page: 1,
        showCompleted: true,
        totalBids: 0,
        totalPages: 1,
    },
    url: String.format("{0}/bids?page=1", App.apiPath),
    initialize: function () {
        this.set('showCompleted', true);
        this.set("bids", new App.Collections.MyBidsCollection());
    },
    setPage: function (value) {
        this.set('page', value);
        this.url = String.format("{0}/bids?page={1}", App.apiPath, value);
    },
    getBids: function () {
        return this.get('bids');
    },
    decreaseBitCount: function () {
        var count = this.get('totalBids') - 1;
        this.set('totalBids', count);
    },
    getCurrentPage: function () {
        return this.get('page');
    },
    getTotalPages: function () {
        return this.get('totalPages');
    },
    parse: function (response) {
        if (App.debug) console.log("User bids: ", response);
        var self = this;

        this.getBids().reset();
        _.each(response.bids, function (bid) {
            var model = new App.Models.BidItemModel(bid);
            model.setTour(new App.Models.ShortTourItemModel(bid.tour));
            if(bid.user) model.setUser(new App.Models.User(bid.user));
            self.getBids().add(model);
        });

        if (response.totalBids>=0) this.set('totalBids', response.totalBids);

        this.set('totalPages', response.totalPages);
        this.set('page', response.page);
    },

});

App.Models.BidItemModel = Backbone.Epoxy.Model.extend({
    defaults: {
        id: '',
        isAccepted: null,
        isWatchedByOwner: false,
        isTourCompleted: false,
        tour: null,
        user: null,
    },
    urlRoot: String.format("{0}/bids",App.apiPath),
    initialize: function () {
        this.blacklist = ['startCity', 'tour', 'user'];
        this.setTour(new App.Models.ShortTourItemModel());
        this.setUser(new App.Models.User());
    },
    save: function (attrs, options) {
        attrs = attrs || this.toJSON();
        options = options || {};
        // If model defines blacklist, exclude them from attrs
        //if (this.blacklist) attrs = _.omit(attrs, this.blacklist);
        if (this.blacklist) attrs = _.omit(attrs, this.blacklist);
        // Move attrs to options
        options.attrs = attrs;

        // Call super with attrs moved to options
        Backbone.Model.prototype.save.call(this, attrs, options);
    },
    accept: function () {
        this.set('isAccepted', true);
        this.set('isWatchedByOwner', true);
    },
    decline: function () {
        this.set('isAccepted', false);
        this.set('isWatchedByOwner', true);
    },
    isAccepted: function () {
        return this.get('isAccepted');
    },
    isWatched: function () {
        return this.get('isWatchedByOwner');
    },
    isCompleted: function () {
        return this.get('isTourCompleted');
    },
    getTour: function () {
        return this.get('tour');
    },
    getUser: function () {
        return this.get('user');
    },
    setTour: function(tour){
        this.set('tour', tour);
    },
    setUser: function(user){
        this.set('user', user);
    },
    computeds: {
        getLink: function () {
            return String.format('#{0}', this.get('id'));
        },
        getRedirectLink: function () {
            return String.format('#/tours/tour/{0}', this.getTour().get('id'));
        },
        getBidOwner: function () {
            return String.format('{0} {1}', this.getUser().get('firstName'), this.getUser().get('lastName'));
        },
        getTourOwner: function () {
            return this.getTour().get('owner');
        },
        getStart: function () {
            return this.get('tour').get('startCity');
        },
        getDest: function () {
            return this.get('tour').get('destCity');
        },
        getSlotsCount: function () {
            return this.get('tour').get('freeSlots');
        },
        getPayment: function () {
            return this.get('tour').get('payment');
        },
        getDate: function () {
            return this.get('tour').get('expirationDate');
        },
    }
});

App.Models.NewBidModel = Backbone.Epoxy.Model.extend({
    defaults: {
        id: '',
    },
    url: function (id) {
        String.format("{0}/bids/addBid?id={1}", App.apiPath, this.get('id'));
    }
});