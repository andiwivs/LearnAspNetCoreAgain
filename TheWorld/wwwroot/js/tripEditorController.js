(function () {

    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {

        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.newStop = {};
        vm.errorMessage = "";
        vm.isBusy = true;

        var apiUrl = "/api/trips/" + vm.tripName + "/stops";

        vm.addStop = function () {

            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post(apiUrl, vm.newStop)
                .then(
                    function (response) {
                        vm.stops.push(response.data);
                        _showMap(vm.stops);
                        vm.newStop = {};
                    },
                    function (error) {
                        vm.errorMessage = "Failed to save new stop: " + error;
                        console.log(error);
                    }
                )
                .finally(
                    function () {
                        vm.isBusy = false;
                    }
                );
        };

        $http.get(apiUrl)
            .then(
                function (response) {
                    angular.copy(response.data, vm.stops);
                    _showMap(vm.stops);
                },
                function (error) {
                    vm.errorMessage = "Failed to load stops: " + error;
                })
            .finally(
                function () {
                    vm.isBusy = false;
                });
    }

    function _showMap(stops) {

        var mapStops = _.map(stops, function (item) {
            return {
                lat: item.latitude,
                long: item.longitude,
                info: item.name
            };
        });

        if (stops && stops.length > 0) {
            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 1,
                initialZoom: 3
            });
        }
    }

})();