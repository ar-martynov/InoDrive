App.Views.MyProfileView = Backbone.Epoxy.View.extend({
    el: '#page',
    template: App.Templates.get('user/myProfile'),

    events: {
        'click #updateProfileBtn': function (e) {
            e.preventDefault();
            this.updateClicked();
        },
        'click #changeEmailBtn': function (e) {
            e.preventDefault();
            this.changeEmailClicked();
        },
        'click #changePasswordBtn': function (e) {
            e.preventDefault();
            this.changePasswordClicked();
        },
        'click #avatarUpload': 'openFile',
        'change #uploadAvatarField': 'uploadFile',
    },

    bindings: {
        "#userAvatar": "attr:{src:userAvatar()}",
        "#firstName": "value:firstName, events:['keyup']",
        "#lastName": "value:lastName, events:['keyup']",
        "#publicEmail": "value:publicEmail, events:['keyup']",
        "#age": "value:age, events:['keyup']",
        "#phone": "value:phone, events:['keyup']",
        "#about": "value:about, events:['keyup']"
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        Backbone.Validation.bind(this);
        var model = this.model;
        this.listenTo(this.model, "change", function () {
            model.isValid(['firstName', 'lastName', 'publicEmail', 'phone', 'age', 'about']);
        });
        this.childViews = [];

        this.changePassword = new App.Views.PasswordChangeView({ model: new App.Models.PasswordChange() });
        this.changeEmail = new App.Views.EmailChangeView({ model: new App.Models.LoginChange() });
        this.childViews.push(this.changePassword, this.EmailChangeView);
        return this;
    },

    stopValidation: function () {
        Backbone.Validation.unbind(this);
    },
    render: function () {
        this.$el.html(this.template);
        this.applyBindings();

        this.changeEmail.$el = this.$('#loginChange');
        this.changeEmail.render();
        this.changePassword.$el = this.$('#passChange');
        this.changePassword.render();

        return this;
    },

    updateClicked: function () {
        if (App.debug) console.log('User profile:', this.model.toJSON());
        if (App.debug) console.log("valid:", this.model.isValid(true))
        if (this.model.isValid(['firsName', 'lastName', 'publicEmail', 'phone', 'age', 'about'])) {
            this.$('#updateProfileBtn').prop('disabled', true);
            var self = this;
            this.model.save({}, {
                url: String.format("{0}/Account/UpdateProfile", App.apiPath),
                success: function (model, response) {
                    self.$('#updateProfileBtn').prop('disabled', false);
                    App.Alerts.showAlert(response.message, "success", true);
                },
                customError: true,
                error: function (model, response) {
                    self.$('#updateProfileBtn').prop('disabled', false);
                    App.Alerts.showAlert( JSON.parse(response.responseText).message, "error", true);
                }
            });
        }

    },
    openFile: function () {
        this.$('#uploadAvatarField').click();
    },

    uploadFile: function () {
        var data = new FormData();
        var thatModel = this.model;
        var that = this;
        var files = this.$("#uploadAvatarField").get(0).files;

        // Add the uploaded image content to the form data collection
        if (files.length > 0) {
            data.append("UploadedImage", files[0]);
            if (files[0].width >350 || files[0].height  > 350) {
                App.Alerts.showAlert("ImageSizeError", "error");
                return ;
            }

            if (!isImage(files[0].name)) {
                App.Alerts.showAlert("FileNotImage", "error");
            } else {
                if (files[0].size < 2048999) {
                    // Make Ajax request with the contentType = false, and procesDate = false
                    $.ajax({
                        type: "POST",
                        url: "/api/upload/avatar",
                        cache: false,
                        contentType: false,
                        processData: false,
                        headers: {
                            "Authorization": App.session.get('token_type') + ' ' + App.session.get('access_token'),
                        },
                        data: data,
                        success: function (data) {
                            if (App.debug) console.log("SUCCESS:: file server path:", data);

                            that.$('#userAvatar').attr("src", data.filePath);
                            thatModel.setAvatar(data.filePath);
                            console.log(thatModel);
                        },
                        error: function (data) {
                            App.Alerts.showAlert(JSON.parse(response.responseText).message, "error", true);
                        }
                    });
                } else App.Alerts.showAlert("FileSizeError", "error");
            }
        }
    },
});



App.Views.UserProfileView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('user/shortUserProfile'),

    bindings: {
        "#userAvatar": "attr:{src:userAvatar()}",
        "#firstName": "text:fullName()",
        "#email": "text:getEmail()",
        "#phone": "text:getPhone()",
        "#age": "text:getAge()",
        "#toursCount": "text:toursCount",
        "#about": "text:getAbout()"
    },

    Initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        this.render();
    },
    render: function () {
        this.$el.html(this.template);

        this.applyBindings();

        this.$('#userRating').raty({ readOnly: true, score: this.model.get('rating') });
        
        return this;
    },
});



App.Views.EmailChangeView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('forms/loginEmailChange'),
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        Backbone.Validation.bind(this);
        return this;
    },

    bindings: {
        "#oldEmail": "value:oldEmail, events:['onChange']",
        "#newEmail": "value:newEmail, events:['onChange']",
    },
    render: function () {
        this.$el.html(this.template);
        this.$el.on('click', '#btn-changeEmail', this.onButtonClick);
        this.applyBindings();
        return this;
    },
    stopValidation: function () {
        Backbone.Validation.unbind(this);
    },
    onButtonClick: function () {
        if (this.model.isValid(['oldEmail', 'newEmail'])) {
            this.$('#btn-changeEmail').button('loading');
            var self = this;
            Backbone.sync("create", this.model, {
                success: function (response) {
                    self.$('#btn-changeEmail').button('reset');
                    App.Alerts.showAlert(response.message, "success", true);
                },
                customError: true,
                error: function (response) {
                    self.$('#btn-changeEmail').button('reset');
                    var error = JSON.parse(response.responseText)['message'];
                    Backbone.Validation.callbacks.invalid(self, 'oldEmail', error);
                    self.model.trigger('validated:invalid', self, 'newEmail', error);
                }
            });
        }
    }
});



App.Views.PasswordChangeView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('forms/passwordChange'),
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        Backbone.Validation.bind(this);
        return this;
    },

    bindings: {
        "#oldPassword": "value:oldPassword, events:['onChange']",
        "#newPassword": "value:newPassword, events:['onChange']",
        "#newPasswordConfirm": "value:newPasswordConfirm, events:['onChange']",
    },
    render: function () {
        this.$el.html(this.template);
        this.$el.on('click', '#btn-changePassword', this.onButtonClick);
        this.applyBindings();
        return this;
    },
    stopValidation: function () {
        Backbone.Validation.unbind(this);
    },
    onButtonClick: function () {
        if (this.model.isValid(['newPassword', 'oldPassword', 'newPasswordConfirm'])) {
            
            this.$('#btn-changePassword').button('loading');
            var self = this;

            this.model.save(null, {
               
                success: function (model, response) {
                    self.$('#btn-changePassword').button('reset');
                    App.Alerts.showAlert( response.message, "success", true);
                },
                customError: true,
                error: function (model, response) {
                    self.$('#btn-changePassword').button('reset');
                    var errors = JSON.parse(response.responseText)['modelState'];
                    if (errors.message[0] == "Incorrect password.") {
                        Backbone.Validation.callbacks.invalid(self, 'oldPassword', 'Неверный пароль');
                        self.model.trigger('validated:invalid', self, 'oldPassword', 'Неверный пароль');
                    }
                }
            });
        }
    }
});