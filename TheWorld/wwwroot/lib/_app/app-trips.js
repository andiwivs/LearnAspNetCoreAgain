!function(){"use strict";angular.module("app-trips",["simpleControls","ngRoute"]).config(["$routeProvider","$locationProvider",function(r,t){r.when("/",{controller:"tripsController",controllerAs:"vm",templateUrl:"/views/trips.html"}),r.when("/editor/:tripName",{controller:"tripEditorController",controllerAs:"vm",templateUrl:"/views/tripEditor.html"}),r.otherwise({redirectTo:"/"}),t.hashPrefix("")}])}();