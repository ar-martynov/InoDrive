App.Models.RegistrationModel = Backbone.Epoxy.Model.extend({
    defaults: {
        firstName: '',
        lastName: '',
        password: '',
        confirmPassword: '',
        userName: '',
    },
    url: String.format("{0}/account/register", App.apiPath),
    validation:
        {
            firstName: {
                required: true,
                msg: 'Пустое поле'
            },
            lastName: {
                required: true,
                msg: 'Пустое поле'
            },
            password: [{
                required: true,
                msg: 'Пустое поле'
            }, {
                minLength: 6,
                msg: 'Пароль короче 6 символов'
            }],
            confirmPassword: function (value) {
                if (value == "") return 'Пустое поле';
                if (value != this.get('password')) {
                    return 'Пароли не совпадают';
                }
            },
            userName: [{
                required: true,
                msg: 'Пустое поле'
            }, {
                pattern: 'email',
                msg: 'Некорректный E-mail'
            }]
        },
});
