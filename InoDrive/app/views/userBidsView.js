App.Views.BidsView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('bids/bids'),
    bindings: {
        "#searchResult": "collection: $collection, itemView: \'itemView\'"
    },
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        this.collection = this.model.getBids();
        this.itemView = App.Views.BidsItemView;
       
    },
    render: function () {
        this.$el.html(this.template);
        this.applyBindings();
        if (this.model.getBids().length == 0) this.$("#searchResult").html(App.Templates.get('system/noBids'))
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

App.Views.UserBidsView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('bids/myBids'),
    bindings: {
        "#searchResult": "collection: $collection, itemView: \'itemView\'",
        "#bidsCount": "text: totalBids",
    },
    events: {
        "click #dontShowCompleted" : "switchCompletedHandler",
    },
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));

        this.collection = this.model.getBids();
        this.collection.bind('remove', this.updateBidsCount);

        this.itemView = App.Views.BaseBidsItemView;
    },
    render: function () {
        this.$el.html(this.template);
        this.applyBindings();
        if (this.model.getBids().length == 0) this.$("#searchResult").html(App.Templates.get('system/noBids'))
        var self = this;
        this.$('#paginator').bootpag({
            total: this.model.getTotalPages() == 1 ? 0 : this.model.getTotalPages(),
            page: this.model.getCurrentPage(),
            maxVisible: 8
        }).on("page", function (event, num) {
            self.model.setPage(num);
            self.model.fetch({
                url: String.format("{0}/bids/MyBids?page={1}&showCompleted={2}", App.apiPath, num, self.model.get('showCompleted')),
            });
        });
        return this;

    },
    updateBidsCount: function () {
        this.model.decreaseBitCount();
    },
    switchCompletedHandler: function (e) {
        this.model.set('showCompleted', !e.currentTarget.checked);
        this.$('#paginator').bootpag({
            page: 1,
            total: 0,
        });
        this.$('#paginator')
        this.model.fetch({
            url: String.format("{0}/bids/MyBids?page=1&showCompleted={1}", App.apiPath, this.model.get('showCompleted'))
        });
    }
});

App.Views.BaseBidsItemView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('bids/userBidCollectionItem'),
    initialize: function () {
        this.$el.append(this.template);
        if (this.model.get('freeSlots') == 0) this.$("#slots").attr('class', 'text-danger');
        this.applyBindings();
        this.initTourStatus();
        return this;
    },

    events: {
        "click .link-rejectMyBid": "rejectMyBidHandler",
    },

    bindings: {
        ".collapse-link": "attr:{\'href\':  getRedirectLink()}",
        "#ownerName": "text: getTourOwner()",
        "#startName": "text: getStart()",
        "#destName": "text: getDest()",
        "#freeSlots ": "text:getSlotsCount()",
        "#expirationDate": "text: getDate()",
        "#payment": "text: getPayment()",
    },
    initTourStatus: function () {
        if (this.model.isAccepted() != null) this.model.isAccepted() ? this.itemAcceptedStyle() : this.itemRejectStyle();
        else {
            if (!this.model.isWatched()) this.itemUnWatchedStyle();
            else this.itemWatchedStyle();
        }
        if (this.model.isCompleted()) this.itemIsCompleted();
    },
    itemUnWatchedStyle: function () {
        this.$(".bid-status").text("Заявка в рассмотрении!");
    },

    itemIsCompleted: function () {
        this.$(".bid-status").text("Поездка завершена!");
    },

    itemWatchedStyle: function () {
        this.$(".bid-status").text("Заявка в рассмотрении!");
    },

    itemAcceptedStyle: function () {
        this.$(".collapse-link").removeClass('list-group-item-warning');
        this.$(".link-reject").removeAttr('disabled');
        this.$(".bid-status").removeClass("text-warning");

        this.$(".collapse-link").addClass('list-group-item-success');
        this.$(".bid-status").addClass("text-success").text("Заявка одобрена!");
        this.$(".link-accept").attr('disabled', true);
    },

    itemRejectStyle: function () {
        this.$(".collapse-link").removeClass('list-group-item-success');
        this.$(".link-accept").removeAttr('disabled');
        this.$(".bid-status").addClass("text-succes")

        this.$(".collapse-link").addClass('list-group-item-warning');
        this.$(".bid-status").addClass("text-warning").text("Заявка отклонена!");
        this.$(".link-reject").attr('disabled', true);
    },

    rejectMyBidHandler: function (e) {
        this.model.destroy({
            success: function () {
                App.Alerts.showAlert("BidRejected", "success");
            }
        });
    },
});


App.Views.BidsItemView = App.Views.BaseBidsItemView.extend({
    template: App.Templates.get('bids/bidCollectionItem'),
    initialize: function () {
        this.$el.append(this.template);
        if (this.model.get('freeSlots') == 0) this.$("#slots").attr('class', 'text-danger');
        this.applyBindings();

        this.userView = new App.Views.UserProfileView({ model: this.model.getUser(), el: this.$('.collapse') });
        this.userView.template = App.Templates.get('user/bidUserProfile');

        this.childViews = [];
        this.childViews.push(this.tourView, this.userView);

        this.userView.render();

        this.initTourStatus();
        //if (this.model.isAccepted() != null) this.model.isAccepted() ? this.itemAcceptedStyle() : this.itemRejectStyle();
       
        return this;
    },
    events: {
        "click .link-accept": "acceptEventHandler",
        "click .link-reject": "declineEventHandler"
    },
    bindings: {
        ".collapse-link": "attr:{\'href\':  getLink()}",
        ".collapse": "attr:{\'id\':  id}",
        "#ownerName": "text: getBidOwner()",
        "#startName": "text: getStart()",
        "#destName": "text: getDest()",
        "#freeSlots ": "text:getSlotsCount()",
        "#expirationDate": "text: getDate()",
        "#payment": "text: getPayment()",
    },
    acceptEventHandler: function (e) {
        var self = this;
        this.model.accept();
        this.model.save(null, {
            success: function (model, response) {
                App.Alerts.showAlert(response.message, "success", true);
                self.itemAcceptedStyle();
                var slotsSpan = self.$("#freeSlots");
                var slotsCount = +slotsSpan.html() - 1;
                slotsSpan.html(slotsCount);
            },
        });
    },
    declineEventHandler: function (e) {
        var self = this;
        this.model.decline();
        this.model.save(null, {
            success: function (model, response) {
                App.Alerts.showAlert(response.message, "warning", true);
                self.itemRejectStyle();
                var slotsSpan = self.$("#freeSlots");
                var slotsCount = +slotsSpan.html() + 1;
                slotsSpan.html(slotsCount);
            },
        });
    },
});