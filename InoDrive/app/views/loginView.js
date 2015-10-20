App.Views.LoginView = Backbone.Epoxy.View.extend({
    id: 'login-modal',
    className: 'modal fade ',
    template: App.Templates.get('login'),
    events: {
        'hidden.bs.modal': 'teardown',
        'click #login': function (e) {
            e.preventDefault();
            this.submitClicked();
        },
        'click #restorePassword': function (e) {
            e.preventDefault();
            this.$('#loginForm').html(App.Templates.get('forms/passwordRestore'));
            this.applyBindings();
        },
        'click #backToLogin': function (e) {
            e.preventDefault();
            this.$('#loginForm').html(App.Templates.get('forms/backToLogin'));
            this.applyBindings();
        },
        'click #restore': 'restorePasswordClicked',
       
    },

    bindings: {
        "#userName": "value:userName, events:['onChange']",
        "#password": "value:password, events:['onChange']",
        "#rememberMe": "value:rememberMe, events:['onChange']"
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        
        this.render();
    },

    render: function () {
        this.$el.html(this.template);
        Backbone.Validation.bind(this);
        this.applyBindings();
        
        return this;
    },

    restorePasswordClicked: function(){
        
        if (this.model.isValid(['userName'])) {
            var self = this;
            var restoreModel = new App.Models.RestorePasswordModel({ userName: this.model.get('userName') });
            this.$('#restore').prop('disabled', true);
            restoreModel.save(null, {
                success: function (model, response) {
                    App.Alerts.showAlert(response.message, 'success', true);
                    self.teardown();
                },
                customError: true,
                error: function (model, response) {
                    self.$('#restore').prop('disabled', false);
                    App.Alerts.showAlert(JSON.parse(response.responseText).message, 'error', true);
                }
            });
        }
    },
        
    submitClicked: function () {
        //this.$('#login').attr('disabled', true);
        if (this.model.isValid(['userName', 'password'])) {
            App.session.postAuth(this.model);
        }
    },
    serverError: function (msg) {
        if (App.debug) console.log("SERVER ERROR MODEL", msg);
    },
    
    show: function () {
        this.$el.modal('show');
    },

    teardown: function () {
        this.$el.modal('hide');
        this.$el.empty();
    }
});