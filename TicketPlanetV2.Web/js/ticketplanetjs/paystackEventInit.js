$(function () {



    function InitTransaction(data) {
        return $.ajax({
            type: "POST",
            url: "/Events/InitializeEventPayment",
            data: data,
            dataType: 'json',
            contentType: 'application/json;charset=utf-8'
        });
    }


    $("#IntializeEventPayment").click(function (e) {
        if ($("#IsFreeEvent").val() == "N") {

            if ($("#IsEventName").val() == "ALHAJI") {
                if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                $('#email').val() === "" ||
                $('#NoOfPersons').val() === "" ||
                $('#TicketCategory').val() === "" ||
                $('#Amount').val() === "" ||
                $('#comments').val() === "") {

                    alertify.error("One or Two Compulsory Fields is Empty");
                    return;
                }
                if ($('#date_preferred').val() === "") {
                    alertify.error("Please select your preferred date");
                    return;
                }
            }
            else {
                if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                $('#email').val() === "" ||
                $('#NoOfPersons').val() === "" ||
                $('#TicketCategory').val() === "" ||
                $('#Amount').val() === "" ||
                $('#comments').val() === "") {

                    alertify.error("One or Two Compulsory Fields is Empty");
                    return;
                }
            }

        }
        else {
            if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                $('#email').val() === "" ||
                $('#NoOfPersons').val() === "") {

                alertify.error("One or Two Compulsory Fields is Empty");
                return;
            }
        }

        $("#status").fadeIn();
        $("#preloader").delay(350).fadeIn("slow");
        e.preventDefault();
        //console.log("date prefered");
        //console.log($("#date_preferred").val());
        var data = JSON.stringify({
            Fullname: $("#Fullname").val(),
            phoneNo: $("#phoneNo").val(),
            email: $("#email").val(),
            NoOfPersons: $("#NoOfPersons").val(),
            TicketCategory: $("#TicketCategory").val(),
            Amount: $("#Amount").val(),
            comments: $("#comments").val(),
            IsFreeEvent: $("#IsFreeEvent").val(),
            TicketType: $("#TicketType").val(),
            TicketCategoryName: $("#TicketCategory>option:selected").text(),
            CouponValue: $("#CouponValue").val(),
            Validated: "N",
            ReferalId: $("#referalId").val(),
            DatePrefered: $("#date_preferred").val()
        });

        $.when(InitTransaction(data)).then(function (response) {


            if (response.error == false) {

                window.location.href = response.result.data.authorization_url;
                //console.log(response.result.data);
            } else {
                $("#status").fadeOut();
                $("#preloader").delay(350).fadeOut("slow");
            }

        }).fail(function () {
            $("#status").fadeOut();
            $("#preloader").delay(350).fadeOut("slow");
        });

    });

    $("#IntializeFreePayPayment").click(function (e) {
        //  alert("test")
        if ($("#IsFreeEvent").val() == "N") {

            if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                $('#email').val() === "" ||
                $('#NoOfPersons').val() === "" ||
                $('#TicketCategory').val() === "" ||
                $('#Amount').val() === "" ||
                $('#comments').val() === "") {

                alertify.error("One or Two Compulsory Fields is Empty");
                return;
            }

        }
        else {
            if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                $('#email').val() === "" ||
                $('#NoOfPersons').val() === "") {

                alertify.error("One or Two Compulsory Fields is Empty");
                return;
            }


        }

        $("#status").fadeIn();
        $("#preloader").delay(350).fadeIn("slow");
        e.preventDefault();

        var data = JSON.stringify({
            Fullname: $("#Fullname").val(),
            phoneNo: $("#phoneNo").val(),
            email: $("#email").val(),
            NoOfPersons: $("#NoOfPersons").val(),
            TicketCategory: $("#TicketCategory").val(),
            Amount: $("#Amount").val(),
            comments: $("#comments").val(),
            IsFreeEvent: $("#IsFreeEvent").val(),
            TicketType: $("#TicketType").val(),
            TicketCategoryName: $("#TicketCategory>option:selected").text(),
            CouponValue: $("#CouponValue").val()

        });
        $.ajax({
            url: "/Events/InitializeFreeEventPayment",
            data: { Fullname: $("#Fullname").val(), phoneNo: $("#phoneNo").val(), email: $("#email").val(), NoOfPersons: $("#NoOfPersons").val(), TicketType: $("#TicketType").val() },
            dataType: "json",
            type: "POST",

            success: function (data) {
                $("#status").fadeOut();
                $("#preloader").delay(350).fadeOut("slow");
                if (data.nErrorCode == 0) {
                    alert(data.sErrorText)
                    $("#Fullname").val()
                    $("#phoneNo").val()
                    $("#email").val()
                    $("#NoOfPersons").val()
                }
                else {
                    alert(data.sErrorText)
                }
            }
        });

    });

});