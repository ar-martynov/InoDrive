App.Views.AdvancedSearchTourProfileView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('tours/tourAdvancedSearch'),

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
    },
    bindings: {
        "#comfort": "value:comfort, events:['onChange']",
        "#baggage": "value:baggage, events:['onChange']",
        "#isDeviationAllowed": "checked:isDeviationAllowed",
        "#isPetsAllowed": "checked:isPetsAllowed",
        "#isMusicAllowed": "checked:isMusicAllowed",
        "#isFoodAllowed": "checked:isFoodAllowed",
        "#isDrinkAllowed": "checked:isDrinkAllowed",
        "#isSmokeAllowed": "checked:isSmokeAllowed",
    },

    render: function () {
        this.$el.append(this.template);

        this.applyBindings();

        var selfModel = this.model;

        this.$('#priceSlider').ionRangeSlider({
            type: "double",
            keyboard: true,
            from: selfModel.getLowerBound(),
            to: selfModel.getUpperBound(),
            min: 0,
            max: 5000,
            grid: true
        }).on("change", function () {
            var $this = $(this),
                value = $this.prop("value").split(";");
            selfModel.setLowerBound(value[0]);
            selfModel.setUpperBound(value[1]);
        });

        return this;
    },

});