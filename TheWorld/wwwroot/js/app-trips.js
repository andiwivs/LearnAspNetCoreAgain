(function () {

    "use strict";

    angular.module("app-trips", ["simpleControls", "ngRoute"])
        .config(function ($routeProvider, $locationProvider) {

            $routeProvider.when("/", {
                controller: "tripsController",
                controllerAs: "vm",
                templateUrl: "/views/trips.html"
            });

            $routeProvider.when("/editor/:tripName", {
                controller: "tripEditorController",
                controllerAs: "vm",
                templateUrl: "/views/tripEditor.html"
            });

            $routeProvider.otherwise({
                redirectTo: "/"
            });

            $locationProvider.hashPrefix(''); // remove "!" added in anglar 1.6
        });
})();