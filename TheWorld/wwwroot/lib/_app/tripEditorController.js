!function(){"use strict";function t(t,n){var s=this;s.tripName=t.tripName,s.stops=[],s.newStop={},s.errorMessage="",s.isBusy=!0;var e="/api/trips/"+s.tripName+"/stops";s.addStop=function(){s.isBusy=!0,s.errorMessage="",n.post(e,s.newStop).then(function(t){s.stops.push(t.data),o(s.stops),s.newStop={}},function(t){s.errorMessage="Failed to save new stop: "+t,console.log(t)}).finally(function(){s.isBusy=!1})},n.get(e).then(function(t){angular.copy(t.data,s.stops),o(s.stops)},function(t){s.errorMessage="Failed to load stops: "+t}).finally(function(){s.isBusy=!1})}function o(t){var o=_.map(t,function(t){return{lat:t.latitude,long:t.longitude,info:t.name}});t&&t.length>0&&travelMap.createMap({stops:o,selector:"#map",currentStop:1,initialZoom:3})}t.$inject=["$routeParams","$http"],angular.module("app-trips").controller("tripEditorController",t)}();