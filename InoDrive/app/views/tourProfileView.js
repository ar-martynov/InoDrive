App.Views.TourProfileView = Backbone.Epoxy.View.extend({
    template: App.Templates.get('tours/tourProfile'),

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
    },

    bindings: {

        "#carImageLink": "attr:{href:carImage}",
        "#carImage": "attr:{src:carImage}",
        "#carDescription": "value:carDescription, events:['onChange']",
        "#ownerExperience": "value:ownerExperience, events:['onChange']",
        "#comfort": "value:comfort, events:['onChange']",
        "#baggage": "value:baggage, events:['onChange']",
        "#isDeviationAllowed": "checked:isDeviationAllowed",
        "#isPetsAllowed": "checked:isPetsAllowed",
        "#isMusicAllowed": "checked:isMusicAllowed",
        "#isFoodAllowed": "checked:isFoodAllowed",
        "#isDrinkAllowed": "checked:isDrinkAllowed",
        "#isSmokeAllowed": "checked:isSmokeAllowed",
        "#additionalInfo": "value:additionalInfo, events:['onChange']"
    },

    render: function () {
        this.$el.append(this.template);
        this.applyBindings();
        if (this.model.get('carImage') == null || this.model.get('carImage') == undefined) this.$('#carImage').attr('src', 'Content/images/no-car.png');

        this.$el.on('click', '#fileUpload', this.openFile);
        this.$el.on('click', '#deleteCarImage', this.deleteCarImage);
        this.$el.on('change', '#uploadField', this.uploadFile);
       
        return this;
    },

    deleteCarImage: function () {
        this.$('#carImage').attr('src', 'Content/images/no-car.png');
        this.$('#carImageLink').attr('href', 'Content/images/no-car.png');
        this.$('#carImageLink').attr('data-title', 'Фото не загружено.');

    },
    openFile: function () {
        this.$('#uploadField').click();
    },

    uploadFile: function () {
        var data = new FormData();
        var self = this;
        var files = this.$("#uploadField").get(0).files;

        // Add the uploaded image content to the form data collection
        if (files.length > 0) {
            data.append("UploadedImage", files[0]);

            if (!isImage(files[0].name)) {
                App.Alerts.showAlert("FileNotImage", "error");
            } else {
                if (files[0].size < 2048999) {
                    // Make Ajax request with the contentType = false, and procesDate = false
                    $.ajax({
                        type: "POST",
                        url: "/api/upload/car",
                        cache: false,
                        contentType: false,
                        processData: false,
                        headers: {
                            "Authorization": App.session.get('token_type') + ' ' + App.session.get('access_token'),
                        },
                        data: data,
                        success: function (response) {
                            if (App.debug) console.log("UPLOADED :: file server path:", response);
                            self.$('#carImage').attr("src", response.filePath);
                            self.model.set('carImage', response.filePath);
                            self.$('#carImageLink').attr('data-title', 'Фото автомобиля, на котором осуществляется поездка.');
                        },
                        error: function (response) {
                            App.Alerts.showAlert(JSON.parse(response.responseText).message, "error", true);
                        }
                    });
                } else App.Alerts.showAlert("FileSizeError", "error");
            }
        }
    },
});
