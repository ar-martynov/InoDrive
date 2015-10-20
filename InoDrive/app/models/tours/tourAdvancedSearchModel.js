App.Models.TourAdvancedSearchModel = Backbone.Epoxy.Model.extend({
    defaults: {
        wayPoints: null,
        paymentLowerBound: 0,
        paymentUpperBound: 5000,
        isDeviationAllowed: false,
        isPetsAllowed: false,
        isMusicAllowed: false,
        isFoodAllowed: false,
        isDrinkAllowed: false,
        isSmokeAllowed: false,
        comfort: 1,
        baggage: 0,
    },
    initialize: function () {
        this.set('wayPoints', new App.Collections.WayPoints());
    },
    wayPoints: function () {
        return this.get('wayPoints');
    },
    setLowerBound: function (value) {
        this.set('paymentLowerBound', value);
    },
    setUpperBound: function (value) {
        this.set('paymentUpperBound', value);
    },
    getLowerBound: function () {
        return this.get('paymentLowerBound');
    },
    getUpperBound: function () {
        return this.get('paymentUpperBound');
    },
});