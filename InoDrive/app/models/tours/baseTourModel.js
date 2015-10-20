App.Models.BaseTourModel = Backbone.Epoxy.Model.extend({
    defaults: {
        expirationDate: '',
        freeSlots: '',
        startCity: '',
        destCity: '',
        payment: '',
    },
    initialize: function () {
        this.set('startCity', new App.Models.CityModel());
        this.set('destCity', new App.Models.CityModel());
    },
    url: String.format("{0}/tours/search", App.apiPath),

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

    validation:
        {
            expirationDate: function (value) {
                if (value == "") return 'Пустое поле';
                var dateFormat = "DD/MM/YYYY";
                var date = moment(value, dateFormat);
                if (!moment(date, dateFormat).isValid() || date.diff(moment(), 'days') < 0) return 'Некорректная дата';
            },
            freeSlots: [{
                required: true,
                msg: 'Пустое поле'
            }, {
                range: [1, 8],
                msg: 'Недопустимое количество пассажиров'
            }],
            startCity: function () {
                if (this.start().isEmpty()) return 'Пункт отправления не указан';
            },
            destCity: function () {
                if (this.dest().isEmpty()) return 'Пункт прибытия не указан';
            },
            payment: function (value) {
                if (value < 0 && isFinite(val) || value > 5000) return 'Недопустимое значение';
            },
        },

    computeds: {
        start: function () {
            return this.get('startCity');
        },
        dest: function () {
            return this.get('destCity');
        }
    },
    payment: function () {
        this.get('payment');
    },
    expirationDate: function () {
        this.get('expirationDate');
    },
    start: function () {
        return this.get('startCity');
    },
    dest: function () {
        return this.get('destCity');
    }
});