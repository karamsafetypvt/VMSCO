
/// <reference path="angular.min.js" />
/// <reference path="angucomplete-alt.js" />
/// <reference path="angular-route.js" />
var app = angular.module('app', []);

app.directive('timer', function ($timeout, $compile) {
    return {
        restrict: 'E',
        scope: {
            interval: '=', //don't need to write word again, if property name matches HTML attribute name
            startTimeAttr: '=?startTime', //a question mark makes it optional
            countdownAttr: '=?countdown' //what unit?
        },
        template: '<div><p>' +
            '<p>Time ends in :{{ minutes }} minutes, ' +
            '{{ seconds }} seconds ' +
            '<span data-ng-if="millis">',

        link: function (scope, elem, attrs) {
            debugger
            //Properties
            scope.startTime = scope.startTimeAttr ? new Date(scope.startTimeAttr) : new Date();
            var countdown = (scope.countdownAttr && parseInt(scope.countdownAttr, 10) > 0) ? parseInt(scope.countdownAttr, 10) : 15; //defaults to 60 seconds
            function tick() {
                //How many milliseconds have passed: current time - start time
                scope.millis = new Date() - scope.startTime;
                if (countdown > 0) {
                    scope.millis = countdown * 1000;
                    countdown--;
                } else if (countdown <= 0) {
                    //alert('Time Up')
                    //start();
                    //resetInterval();
                    scope.stop();
                    console.log('Your time is up!');
                }

                scope.seconds = Math.floor((scope.millis / 1000) % 60);
                scope.minutes = Math.floor(((scope.millis / (1000 * 60)) % 60));
                scope.hours = Math.floor(((scope.millis / (1000 * 60 * 60)) % 24));
                scope.days = Math.floor(((scope.millis / (1000 * 60 * 60)) / 24));

                //is this necessary? is there another piece of unposted code using this?
                scope.$emit('timer-tick', {
                    intervalId: scope.intervalId,
                    millis: scope.millis
                });

                scope.$apply();

            }

            function resetInterval() {
                if (scope.intervalId) {
                    clearInterval(scope.intervalId);
                    scope.intervalId = null;
                }
            }
            scope.stop = function () {
                scope.stoppedTime = new Date();
                resetInterval();
            }

            //if not used anywhere, make it a regular function so you don't pollute the scope
            function start() {
                resetInterval();
                scope.intervalId = setInterval(tick, scope.interval);
            }

            scope.resume = function () {
                scope.stoppedTime = null;
                scope.startTime = new Date() - (scope.stoppedTime - scope.startTime);
                start();
            }

            start(); //start timer automatically

            //Watches
            scope.$on('time-start', function () {
                start();
            });

            scope.$on('timer-resume', function () {
                scope.resume();
            });

            scope.$on('timer-stop', function () {
                scope.stop();
            });

            //Cleanup
            elem.on('$destroy', function () {
                resetInterval();
            });
        }
    };
});