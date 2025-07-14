/// <reference path="angular.min.js" />
/// <reference path="angular-route.js" />

var angualarModule = angular.module("angualarModule", ['ngRoute']);
angualarModule.config(function ($routeProvider) {
    $routeProvider.
        when('/firstPage', {
            templateUrl: 'routedemo/first',
            controller: 'routeDemoFirstController'
        }).
        when('/secondPage', {
            templateUrl: 'routedemo/second',
            controller: 'routeDemoSecondController'
        })
})

angualarModule.controller("AppointmentController", function ($scope, $http) {
    $scope.MaterialData = function () {
        $http({
            method: 'GET',
            url: '/Material/OutWardSecurity'
        }).then(function (response) {
            debugger
            $scope.SiteData = response.data;
        })
    }

    $scope.getselectval = function () {
        debugger
        var ID = $scope.selitem
        if (ID != null) {
            $http({
                method: 'GET',
                params: { ID: ID },
                url: '/Home/GetVisitorData'
            }).then(function (response) {
                $scope.VisitorData = response.data
            })
        }
    }

    $scope.btntext = "Save";
    $scope.savedata = function () {        
        var Company = {
            CName: $scope.Company.CName,
            Addres: $scope.Company.Addres,
            Phone: $scope.Company.Phone,
            GST: $scope.Company.GST,
            city: $scope.Company.city,
            state: $scope.Company.state,
            PIN: $scope.Company.PIN
        };
        $scope.btntext = "Please wait...!";




        $http({
            method: 'POST',
            url: '/Home/Register',
            data: JSON.stringify(Company)
        }).then(function (response) {
            debugger
            var d = $("#fVisitorCompanyName").val()
            //alert(d)      
            var s = "<option selected value='" + response.data + "'>" + d + "</option>";
            $("#CompName").append(s);
            $("#HVisitorCompanyID").val(response.data);
            //$("#CompName").val(d)
            //window.location.href = '/Home/Create'
            $(".dialog-form").dialog("close");
        })
    }

    $scope.VendorAdd = function () {
        var Company = {
            CName: $scope.Company.CName,
            Addres: $scope.Company.Addres,
            GST: $scope.Company.GST
        };
        $scope.btntext = "Please wait...!";
        $http({
            method: 'POST',
            url: '/Material/VendorAdd',
            data: JSON.stringify(Company)
        }).then(function (response) {
            alert('Vendor added')
            $(".dialog-formVendor").dialog("close");
        })
    }

    //$scope.VisitorData = function () {
    //    debugger
    //    var x = angular.element(document.getElementById("HVisitorCompanyID"));
    //    var Visitor = {
    //        V_Name: $scope.Visitor.V_Name,
    //        C_ID: x.val(),
    //        V_Phone: $scope.Visitor.V_Phone,
    //        V_Email: $scope.Visitor.V_Email,
    //        Visitor_ID: $scope.Visitor.Visitor_ID,
    //        V_IDNumber: $scope.Visitor.V_IDNumber
    //    };
    //    $http({
    //        method: 'POST',
    //        url: '/Home/RegisterVisitor',
    //        data: JSON.stringify(Visitor)
    //    }).then(function (response) {
    //        $http({
    //            method: 'GET',
    //            params: { ID: x.val() },
    //            url: '/Home/GetVisitorData'
    //        }).then(function (response) {
    //            $scope.VisitorData = response.data
    //        })
    //        $(".dialog-formVisitorAdd").dialog("close");
    //        window.location.href = '/Home/Create'
    //    })
    //};
    $scope.VisitorData = function () {
        debugger
        //alert('tt');
        var x = angular.element(document.getElementById("HVisitorCompanyID"));
        var y = angular.element(document.getElementById("VisIDType"));

        
       /// var IDcheckvalidation = angular.element(document.getElementById("IDcheckvalidation"));
        var CompName = angular.element(document.getElementById("CompName"));
        

        var VType = "Visitor";
        var Visitor = {
            V_Name: $scope.Visitor.V_Name,
            C_ID: x.val(),
            V_Phone: $scope.Visitor.V_Phone,
            V_Email: $scope.Visitor.V_Email,
            Visitor_ID: y.val(),
            V_IDNumber: $scope.Visitor.V_IDNumber,
            V_Type: VType


        };

        //---------------dec23----------
        //-------Addedjitendra dec21---------
        //alert(Visitor.C_ID);

        //----------------------comment jan26

        var check = "1";
        //var check = IDcheckvalidation.val();
        //alert(CompName.val());
        if (Visitor.C_ID == null || Visitor.C_ID == "" || Visitor.C_ID == "undefined") {
            alert('Please Select  Company');
            check = "0";
            return false;
        }

      

        else if (CompName.val() == "Select One") {
            alert('Please Select  Company');
            check = "0";
            return false;
        }




        else if (Visitor.V_Name == null || Visitor.V_Name == "" || Visitor.V_Name == "undefined") {
            alert('Please Enter Name');
            check = "0";
            return false;
        }

        else if (Visitor.V_Phone == null || Visitor.V_Phone == "" || Visitor.V_Phone == "undefined") {
            /*  alert('Please add 10 digit mobile number');*/
            alert('Please enter mobile number');
            check = "0";
            return false;
        }
        else if (Visitor.V_Phone.length < 2) {
            /* alert('Please enter 10 digit mobile number')*/
            alert('Please enter correct mobile number')
            check = "0";
            return false;
        }

        //else if (Visitor.V_Email == null || Visitor.V_Email == "" || Visitor.V_Email == "undefined") {
        //    alert('Please Enter Email123');
        //    check = "0";
        //    return false;
        //}




        else if (y.val() == "? object:null ?") {
            alert('Please Select Id Type');
            check = "0";
            return false;
        }

        else if (Visitor.V_IDNumber == null || Visitor.V_IDNumber == "" || Visitor.V_IDNumber == "undefined") {
            alert('Please Enter Id number')
            check = "0";
            return false;
        }

//---------comment jan26

        //-------------
        //dec23------------------

        //alert('kkkk');
        //if (Visitor.V_Phone == null || Visitor.V_Phone == "" || Visitor.V_Phone == "undefined") {
        //    alert('Please add number')
        //    return false;
        //}
        //else if(Visitor.V_Phone.length < 10)
        //{
        //    alert('Please enter 10 digit mobile number101')
        //    return false;
        //}
       else if (check == "1") {
                //  if (check == "1") {

            $http({
                method: 'POST',
                url: '/Home/RegisterVisitor',
                data: JSON.stringify(Visitor)
            }).then(function (response) {
                debugger
                var c = $("#CompName").val();
                var d = $("#CompName option:selected").text();
                var s = "<option selected value='" + c + "'>" + d + "</option>";
                $("#divCam").append(s);
                $("#DivVC").append(s);
                var n = $("#fvVisitorName").val();
                var vn = "<option selected value='" + response.data + "'>" + n + "</option>";
                $("#divVis").append(vn);
                $("#divVisS").append(vn);

                var m = $("#fvVisitorPhone").val();
                $("#MBNumber").val(m);
                $("#SecMblNumber").val(m);

                var e = $("#fvVisitorEmail").val();

                var idT = $("#VisIDType").val();
                var dt = "<option selected >" + idT + "</option>";
                $("#divID").append(dt);
                $("#IDProofType").append(dt);

                var idn = $("#IDNumberP").val();
                $("#IDNumber").val(idn);
                $("#V_IDNumber").val(idn);
                $http({
                    method: 'GET',
                    params: { ID: x.val() },
                    url: '/Home/GetVisitorData'
                }).then(function (response) {
                    $scope.VisitorData = response.data
                })
                $(".dialog-formVisitorAdd").dialog("close");
            })
        }
        else {

            alert('Please Enter Correct data');
        }
    };
})
angualarModule.controller('myCtrl', function ($scope, $interval, $window, $http) {
    var qs = $window.location.search;
    var q = qs.split('=');
    debugger
    $scope.counter1 = "";
    $scope.functTime = function () {
        $http({
            method: "GET",
            params: { ID: q[1] },
            url: '/Material/SecurityTime'
        }).then(function (result) {
            $scope.counter1 = result.data;
        })
    }

    $scope.counter = $scope.counter1;
    $interval(function () {
        if ($scope.counter == 0) {
            $scope.counter = $scope.counter1;
            //$scope.counter = 10;
        }
        else {
            $scope.counter--
        }
    }, 1000);

    $scope.ApproveLevelOne = function () {
        debugger
        var qs = $window.location.search;
        var q = qs.split('=');
        $http({
            method: "GET",
            params: { challan: q[1] },
            url: '/Material/GetOutwardData'
        }).then(function (result) {
            $scope.OutWardData = result.data            
        })
    }
});

angualarModule.filter('counter', [function () {
    return function (seconds) {
        return new Date(2020, 0, 1).setSeconds(seconds);
    };
}])
