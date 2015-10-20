App.Models.TourProfileModel = Backbone.Epoxy.Model.extend({
    defaults: {
        isDeviationAllowed: false,
        isPetsAllowed: false,
        isMusicAllowed: false,
        isFoodAllowed: false,
        isDrinkAllowed: false,
        isSmokeAllowed: false,
        carDescription: '',
        carImage: 'Content/images/no-car.png',
        carImageExtension: '',
        comfort: 1,
        baggage: 0,
        additionalInfo: '',
        ownerExperience: '',
    },
    computeds: {
        image: {
            get: function () {
                return this.get("carImage");
            },
            set: function (value) {
                alert(value);
                return this.set("carImage", value);
            }
        }
    }
});