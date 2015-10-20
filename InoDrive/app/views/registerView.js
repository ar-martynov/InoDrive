App.Views.RegistrationView = Backbone.Epoxy.View.extend({
    id: 'newUser-modal',
    className: 'modal fade ',
    template: App.Templates.get('registration'),
    events: {
        'hidden.bs.modal': 'teardown',
        'click #register': function (e) {
            e.preventDefault();
            this.submitClicked();
        }
    },

    bindings: {
        "#userName": "value:userName, events:['onChange']",
        "#firstName": "value:firstName, events:['onChange']",
        "#lastName": "value:lastName, events:['onChange']",
        "#password": "value:password, events:['onChange']",
        "#confirmPassword": "value:confirmPassword, events:['onChange']"
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

    submitClicked: function () {
        // Check if the model is valid before saving
        // See: http://thedersen.com/projects/backbone-validation/#methods/isvalid
        if (this.model.isValid(['userName', 'password', 'firstName', 'lastName', 'confirmPassword', 'email'])) {

            this.model.save(null, {
                success: function (response) {
                    App.Views.CurrentModalView.teardown();
                    App.Alerts.showAlert("Authorization", "success");
                },
                customError: true,
                error: function (response) {
                    var errors = 'E-mail уже используется';
                    Backbone.Validation.callbacks.invalid(App.Views.CurrentModalView, 'userName', errors);
                    App.Views.CurrentModalView.model.trigger('validated:invalid', App.Views.CurrentModalView, 'userName', errors);
                }
            });
        }
    },

    serverError: function (msg) {
        console.log("SERVER ERROR MODEL", msg);
    },

    show: function () {
        this.$el.modal('show');
    },

    teardown: function () {
        this.$el.modal('hide');
        this.$el.empty();
    }
});