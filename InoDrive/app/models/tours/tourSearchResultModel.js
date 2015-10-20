App.Models.TourSearchResultModel = App.Models.TourModel.extend({
    defaults: {
        id: '',
        isExpired: false,
        isOwner: false,
        alreadyBet: false,
        expirationDate: '',
        freeSlots: '',
        payment: 0,
        startCity: null,
        destCity: null,
        wayPoints: null,
        tourProfile: null,
        userProfile: null,
        
    },
    url: function () {
        return String.format("{0}/tours/get?id={1}", App.apiPath, this.get('id'));
    },
    initialize: function () {
        this.set('wayPoints', new App.Collections.WayPoints());
        this.set('tourProfile', new App.Models.TourProfileModel());
        this.set('startCity', new App.Models.CityModel());
        this.set('destCity', new App.Models.CityModel());
    },
    wayPoints: function () {
        return this.get('wayPoints');
    },
    getUserProfile: function () {
        return this.get('userProfile');
    },
    parse: function (response, options) {
        if (App.debug) console.log('App.Models.TourSearchResultModel FETCHED:', response);

        this.set('tourProfile', new App.Models.TourProfileModel(response.tourProfile));
        this.set('userProfile', new App.Models.User(response.userProfile));
        this.set('expirationDate', response.expirationDate);
        this.set('freeSlots', response.freeSlots);
        this.set('payment', response.payment);
        this.set('isOwner', response.isOwner);
        this.set('alreadyBet', response.alreadyBet);
        this.set('isExpired', response.isExpired);
        for (var i = 0; i < response.wayPoints.length; i++) {
            if (i != 0 && i != response.wayPoints.length - 1) this.wayPoints().add(new App.Models.CityModel(response.wayPoints[i]));
            if (i == 0) this.set('startCity', new App.Models.CityModel(response.wayPoints[i]));
            if (i == response.wayPoints.length - 1) this.set('destCity', new App.Models.CityModel(response.wayPoints[i]));

        }
    },
    isCarImageExist: function () {
        var img = this.get('tourProfile').get('carImage');
        if (img == 'Content/images/no-car.png' || img == null) return false;
        return true;
    },
   
    isExpired: function () {
        return this.get('isExpired');
    },
    isOwner: function () {
        return this.get('isOwner');
    },
    alreadyBet: function () {
        return this.get('alreadyBet');
    },
    computeds: {
        carDescription: function () {
            var description = this.get('tourProfile').get('carDescription') == null ? 'не указано' : this.get('tourProfile').get('carDescription');
            return description;
        },
        additionalInfo: function () {
            var description = this.get('tourProfile').get('additionalInfo') == null ? 'не указано' : this.get('tourProfile').get('additionalInfo');
            return description;
        },
        carImage: function () {
            if (this.get('tourProfile').get('carImage') == 'Content/images/no-car.png') return '';
            return this.get('tourProfile').get('carImage');
        },
        start: function () {
            return this.get('startCity').get('cityNameRu');
        },
        dest: function () {
            return this.get('destCity').get('cityNameRu');
        }
    }
});