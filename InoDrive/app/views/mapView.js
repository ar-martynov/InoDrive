App.Views.MapView = Backbone.Epoxy.View.extend({
    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        this.directionsService = new google.maps.DirectionsService();
        this.directionsDisplay = new google.maps.DirectionsRenderer();

        this.listenTo(this.model.start(), "startCityComplete", this.renderMap);
        this.listenTo(this.model.dest(), "destCityComplete", this.renderMap);
        this.listenTo(this.model.wayPoints(), "newWaypoint", this.renderMap);
    },

    render: function () {
        var mapOptions = {
            zoom: 8,
            scrollwheel: true,
            draggable: true,
            disableDefaultUI: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        this.map = new google.maps.Map(document.getElementById(this.el), mapOptions);
        //this.initializeMapCenter();
        if (!this.model.start().isEmpty() || !this.model.dest().isEmpty()) {
            this.renderMap();
            return this;
        }
    },

    initializeMapCenter: function () {
        // Try HTML5 geolocation
        if (navigator.geolocation) {
            var self = this;
            navigator.geolocation.getCurrentPosition(function (position) {
                var pos = new google.maps.LatLng(position.coords.latitude,
                                                 position.coords.longitude);
                self.map.setCenter(pos);
            });
        } else {
            // Browser doesn't support Geolocation
            self.map.setCenter(new google.maps.LatLng(55, 37));
        }
    },

    renderMap: function () {

        var start = this.model.start();
        var dest = this.model.dest();

        this.checkMarkers(start.isEmpty(), dest.isEmpty());
        // Markers area
        var markersBounds = new google.maps.LatLngBounds();

        if (start.getLat() != '' && start.getLong() != '') {
            var startPosition = new google.maps.LatLng(start.getLat(), start.getLong());
            // Add marker to area
            markersBounds.extend(startPosition);
            //create marker
            this.startMarker = new google.maps.Marker({
                position: startPosition,
                animation: google.maps.Animation.DROP,
                map: this.map,
                title: start.getTitle()
            });
        }

        if (dest.getLat() != '' && dest.getLong() != '') {
            var destPosition = new google.maps.LatLng(dest.getLat(), dest.getLong());
            // Add marker to area
            markersBounds.extend(destPosition);
            //create marker
            this.destMarker = new google.maps.Marker({
                position: destPosition,
                animation: google.maps.Animation.DROP,
                map: this.map,
                title: start.getTitle()
            });
        }

        this.map.setCenter(markersBounds.getCenter());

        //route
        if (!start.isEmpty() && !dest.isEmpty()) {
            this.directionsDisplay.setMap(this.map);

            var request = {
                origin: startPosition,
                destination: destPosition,
                waypoints: this.getWaypoints(),
                optimizeWaypoints: true,
                travelMode: google.maps.TravelMode.DRIVING
            };
            this.checkMarkers(start.isEmpty(), dest.isEmpty());
            this.directionsService.route(request, this.drawRoute);
        }
    },

    getWaypoints: function () {
        var waypts = [];
        this.model.wayPoints().each(function (wayPoint) {
            waypts.push({
                location: new google.maps.LatLng(wayPoint.getLat(), wayPoint.getLong()),
                stopover: true
            });
        });
        return waypts;
    },

    drawRoute: function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            this.directionsDisplay.setDirections(response);
        }
    },

    checkMarkers: function (startStatus, destStatus) {
        if (!startStatus && this.startMarker != undefined) this.startMarker.setMap(null);
        if (!destStatus && this.destMarker != undefined) this.destMarker.setMap(null);
    }

});