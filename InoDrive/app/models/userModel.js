App.Models.User = Backbone.Epoxy.Model.extend({
    defaults: {
        toursCount: 0,
        newBidsCount: '',
        newAdsCount: '',
        rating: '',
        firstName: '',
        lastName: '',
        age: '',
        avatarPath: 'Content/images/no_avatar.png',
        phone: '',
        about: '',
        publicEmail: '',
        unWatchedBidsCount: '',
        unWatchedAdsCount:'',
    },
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
           age: [{
               required: true,
               msg: 'Пустое поле'
           }, {
               range: [1, 110],
               msg: 'Недопустимый возраст'
           }],
           phone: function (value) {
               if (value == '') return "Пустое поле";
               if (value.toString().length != 11) return "Не является номером телефоном";
           },
           publicEmail: [{
               required: true,
               msg: 'Пустое поле'
           }, {
               pattern: 'email',
               msg: 'Некорректный E-mail'
           }],
           about: function (value) {
               if(value.length>200 ) return 'Много информации'
           },
       },
    computeds: {
        notificationsCount: function () {
            if (+this.get('newBidsCount') == 0 && +this.get('newAdsCount') == 0) return '';
            return +this.get('newBidsCount') +  (+this.get('newAdsCount'));
        },
        fullName: function () {
            return this.get("firstName") + " " + this.get("lastName");
        },
        userAvatar: function () {
            if (this.get('avatarPath') == null) return 'Content/images/no_avatar.png';
            return this.get('avatarPath');
        },
        getEmail: function () {
            if (this.get('publicEmail') == null) return '';
            return this.get('publicEmail');
        },
        getPhone: function () {
            if (this.get('phone') == null) return '';
            return this.get('phone');
        },
        getAge: function () {
            if (this.get('age') == 0) return 'не указан';
            return this.get('age');
        },
        getAbout: function () {
            if (this.get('about') == null) return 'нет данных';
            return this.get('about');
        }
    },
    url: function () {
        return String.format("{0}/Account/GetProfile", App.apiPath);
    },
    setAvatar: function (path) {
        this.set("avatarPath", path);
    },
    clearBidsCount: function () {
        this.set('newBidsCount', '');
    },
    clearAdsCount: function () {
        this.set('newAdsCount', '');
    },
    increaseBidsCount: function () {
        var current = + this.get('newBidsCount') + 1;
        this.set('newBidsCount', current);
    },
    decreaseBidsCount: function () {
        var current = this.get('newBidsCount') > 1 ? +this.get('newBidsCount') - 1 : '';
        this.set('newBidsCount', current);
    },
    increaseAdsCount: function () {
        var current = + this.get('newAdsCount') + 1;
        this.set('newAdsCount', current);
    },
    decreaseAdsCount: function () {
        var current = this.get('newAdsCount') > 1 ? +this.get('newAdsCount') - 1 : '';
        this.set('newAdsCount', current);
    },
});

App.Models.PasswordChange = Backbone.Epoxy.Model.extend({
    defaults: {
        oldPassword: '',
        newPassword: '',
        newPasswordConfirm: '',
    },
    validation:
       {
           oldPassword: [{
               required: true,
               msg: 'Пустое поле'
           }, {
               minLength: 6,
               msg: 'Некорректная длина пароля'
           }],
           newPassword: function (value) {
               if (value == "") return 'Пустое поле';
               if (value.length < 6) return 'Некорректная длина пароля';
               if (value != this.get('newPassword')) {
                   return 'Пароли не совпадают';
               }
               if (value == this.get('oldPassword')) return 'Введите новый пароль'
           },
           newPasswordConfirm: function (value) {
               if (value == "") return 'Пустое поле';
               if (value != this.get('newPassword')) {
                   return 'Пароли не совпадают';
               }
           },
       },
    url: function () {
        return String.format("{0}/Account/ChangePassword", App.apiPath);
    },
});

App.Models.LoginChange = Backbone.Epoxy.Model.extend({
    defaults: {
        oldEmail: '',
        newEmail: '',
    },
    validation:
       {
           oldEmail: [{
               required: true,
               msg: 'Пустое поле'
           }, {
               pattern: 'email',
               msg: 'Некорректный E-mail'
           }],
           newEmail: [{
               required: true,
               msg: 'Пустое поле'
           }, {
               pattern: 'email',
               msg: 'Некорректный E-mail'
           }],
       },
    url: function () {
        return String.format("{0}/Account/ChangeLogin", App.apiPath);
    },
});