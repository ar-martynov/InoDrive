App.Views.HeaderView = Backbone.Epoxy.View.extend({
    el: '#header',
    events: {
        "click #logout": "onLogoutClick",
        "click #login": "onLoginClick",
        "click #register": "onRegisterClick"
    },

    bindings: {
        "#fullName": "text: fullName(), ",
        "#allNotifies": "text: notificationsCount, events:['onChange']",
        "#myAds": "text: newAdsCount",
        "#myBids": "text: newBidsCount",
        
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        var self = this;
        this.listenTo(App.session, 'statusChanged', this.onLoginStatusChange);
        this.render();
    },
    render: function () {
        if(App.session.status()) this.$el.html(App.Templates.get('header/logged'));
        else this.$el.html(App.Templates.get('header/notLogged'));
        this.applyBindings();
        return this;
    },

    onLoginStatusChange: function (model) {
        this.render();
        if (App.session.get("logged_in")) App.Alerts.showAlert("LogIn", "success");
        else App.Alerts.showAlert("LogOut", "success");
    },

    onLogoutClick: function () {
        App.session.logout();
        this.render();
    },

    onLoginClick: function () {
        App.Views.DisposeModal();

        App.Views.CurrentModalView = new App.Views.LoginView({
            model: new App.Models.LoginModel()
        });
        App.Views.CurrentModalView.show();
    },

    onRegisterClick: function () {
        App.Views.DisposeModal();

        App.Views.CurrentModalView = new App.Views.RegistrationView({
            model: new App.Models.RegistrationModel()
        });
        App.Views.CurrentModalView.show();
    }
});