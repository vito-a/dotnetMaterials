var app = angular.module('taskApp', []);

app.controller('TaskController', function ($scope, $http) {
    $scope.tasks = [];
    $scope.newTask = {};

    $scope.getTasks = function () {
        $http.get('/api/Tasks').then(function (response) {
            $scope.tasks = response.data;
        });
    };

    $scope.addTask = function () {
        $http.post('/api/Tasks', $scope.newTask).then(function (response) {
            $scope.tasks.push(response.data);
            $scope.newTask = {};
        });
    };

    $scope.deleteTask = function (id) {
        $http.delete('/api/Tasks/' + id).then(function (response) {
            $scope.getTasks();
        });
    };

    $scope.editTask = function (task) {
        $scope.newTask = angular.copy(task);
    };

    $scope.generateXmlReport = function () {
        $http.get('/api/Tasks/XmlReport').then(function (response) {
            var blob = new Blob([response.data], { type: 'application/xml' });
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = 'tasks_report.xml';
            a.click();
            window.URL.revokeObjectURL(url);
        });
    };

    $scope.getTasks();
});
