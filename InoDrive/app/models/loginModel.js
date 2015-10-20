App.Models.LoginModel = Backbone.Epoxy.Model.extend({
    defaults: {
        userName: '',
        password: '',
        rememberMe: false,
        grant_type: 'password',
        client_Id: App.apiClientId,
        client_Secret: App.apiClientSecret,
    },
    validation:
        {
            userName: [{
                required: true,
                msg: 'Пустое поле'
            }, {
                pattern: 'email',
                msg: 'Некорректный E-mail'
            }],
            password: {
                required: true,
                msg: 'Пустое поле',
            },
            rememberMe: {
                required: false,
            }
        },
    url: String.format("{0}/token", App.apiPath),
    getParamString: function (attr) {
        var str = "";
        for (var key in attr) {
            if (str != "") {
                str += "&";
            }
            str += key + "=" + encodeURIComponent(attr[key]);
        }
        return str;
    }
});
App.Models.RestorePasswordModel = Backbone.Epoxy.Model.extend({
    defaults: {
        userName: '',
    },
    validation:
        {
            userName: [{
                required: true,
                msg: 'Пустое поле'
            }, {
                pattern: 'email',
                msg: 'Некорректный E-mail'
            }],
        },
    url: String.format("{0}/account/restore", App.apiPath),
});