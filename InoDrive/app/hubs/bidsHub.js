App.Hubs.BidsHubWrapper = function () {
    var self = this;
    var bids = null;
    var isStarted = false;
    var userId = '';

    var newBidCallBack = function (tourName) {
        var message = String.format('<strong>{0}</strong> <br> новая заявка на поездку!', tourName);

        if (App.Views.CurrentView instanceof App.Views.BidsView) App.Views.CurrentView.model.fetch(null, {
            success: function (model) {
                App.Views.CurrentView.render();
            },
            customError: true,
            error: function () {
            },
        });

        App.session.user.increaseAdsCount();
        App.Alerts.showAlert(message, 'info', true);
    };
    var rejectBidCallBack = function (tourName) {
        var message = String.format('<strong>{0}</strong> <br> заявка отозвана!', tourName);

        if (App.Views.CurrentView instanceof App.Views.BidsView) App.Views.CurrentView.model.fetch(null, {
            success: function (model) {
                App.Views.CurrentView.render();
            },
            customError: true,
            error: function () {
            },
        });

        App.session.user.decreaseAdsCount();    
        App.Alerts.showAlert(message, 'warning', true);
    };

    var bidStatusCallback = function (status) {
        var message = status ? "Ваша заявка одобрена!" : "Ваша заявка отклонена!";
        var statusClass = status ? "success" : "warning";
       

        if (App.Views.CurrentView instanceof App.Views.UserBidsView) {
            App.Views.CurrentView.model.url = App.apiPath + "/bids/myBids?page=" + App.Views.CurrentView.model.get('page') + "&showCompleted=" + App.Views.CurrentView.model.get('showCompleted');
            App.Views.CurrentView.model.fetch(null, {
                success: function (model) {
                    App.Views.CurrentView.render();
                },
                customError: true,
                error: function () {
                },
            });
        } else {
            if (status) App.session.user.increaseBidsCount();
            else App.session.user.decreaseBidsCount();
        }
        App.Alerts.showAlert(message, statusClass, true);
    };

    
    self.start = function ( ) {
        bids = $.connection.Bids;
        bids.client.newBid = newBidCallBack;
        bids.client.rejectBid = rejectBidCallBack;
        bids.client.bidStatus = bidStatusCallback;
        var connection = $.connection.hub;

        connection.stateChanged(connectionStateChanged);

        return connection.start()
            .done(function () {
                isStarted = true;
                userId = $.connection.hub.id;
            })
            .fail(function () {
                if (App.debug) console.log('SignalR :: Could not connect!');
            });
    };

    self.stop = function () {
        isStarted = false;
        bids = null;
        return $.connection.hub.stop();
    };

    function connectionStateChanged(state) {
        var stateConversion = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
        if(App.debug) console.log('SignalR state changed from: ' + stateConversion[state.oldState]
         + ' to: ' + stateConversion[state.newState]);
    }

};