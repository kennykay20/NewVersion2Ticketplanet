$(function () {



    function InitFlwTransactionP(data) {
        return $.ajax({
            type: "POST",
            url: '/Movies/FlutterwaveMoviePaymentPercent',
            data: data,
            dataType: 'json',
            contentType: 'application/json;charset=utf-8'
        });
    }

    $("#IntializeFlwPaymentPercent").click(function (e) {


        //if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
        //         $('#email').val() === "" ||
        //         $('#NoOfPersons').val() === "" ||
        //         $('#MovieCategory').val() === "" ||
        //         $('#Amount').val() === "" || $('#MovieDay').val() === "" || $('#MovieTime').val() === "") {

        //    alertify.error("One or Two Compulsory Fields is Empty");
        //    return;
        //}

        $("#status").fadeIn();
        $("#preloader").delay(350).fadeIn("slow");
        e.preventDefault();
        var totalAmounts = ($("#Amount").val() + $("#amtCharge").val());

        var data = JSON.stringify({
            Fullname: $("#Fullname").val(),
            phoneNo: $("#phoneNo").val(),
            email: $("#email").val(),
            NoOfPersons: $("#NoOfPersons").val(),
            MovieCategory: $("#MovieCategory").val(),
            Amount: $("#Amount").val(),
            comments: $("#comments").val(),

            CinemaLocation: $("#CinemaCompanyID").val(),
            CinemaCompanyID: $("#CinemaCompanyID").val(),

            MovieDay: $("#MovieDay").val(),
            MovieTime: $("#MovieTime").val(),
            MovieName: $("#MovieName").val(),

            IsCoupon: $("#IsCoupon").val(),
            Coupon: $("#Coupon").val(),
            CouponAgentId: $("#CouponAgentId").val(),
            CouponAssignId: $("#CouponAssignId").val(),
            CouponID: $("#CouponID").val(),
            nErrorCode: $("#nErrorCode").val(),
            CouponValue: $("#CouponValue").val()
        });

        $.when(InitFlwTransactionP(data)).then(function (response) {


            if (response.error == false) {
                $("#status").fadeOut();
                $("#preloader").delay(350).fadeOut("slow");
                //console.log(response.result);
                //console.log(('#amtCharge').val());
                //console.log(totalAmounts);
                payWithRave(response.result.publicKey, response.result);
                //window.location.href = response.result.data.authorization_url;

            } else {
                $("#status").fadeOut();
                $("#preloader").delay(350).fadeOut("slow");
            }

        }).fail(function () {
            $("#status").fadeOut();
            $("#preloader").delay(350).fadeOut("slow");
        });

    });

});

//const API_publicKey = "<ADD YOUR PUBLIC KEY HERE>";


function payWithRave(pbKeys, data) {
    //var phone = $('#njj').val();

    //var txtRef = data.Reference;
    var x = getpaidSetup({
        PBFPubKey: pbKeys,
        customer_email: data.email,
        amount: data.amount,
        customer_phone: data.phoneNo,
        customer_fullName: data.firstname,
        currency: "NGN",
        country: "NG",
        txref: data.Reference,
        meta: [{
            metaname: "TICKETPLANET LIMITED",
            metavalue: "1133395"
        }],
        onclose: function () { },
        callback: function (response) {
            var fltRef = response.tx.txRef; // collect txRef returned and pass to a server page to complete status check.
            //console.log("This is the response returned after a charge", response);
            //console.log(fltRef);

            if (response.tx.chargeResponseCode == "00") {
                // redirect to a success page

                //console.log(response.tx.txRef);
                //console.log(response.tx.raveRef);
                //updateFlutterwave, paymentConfirmationFlw
                var flutterRef = response.tx.flwRef;
                var tkReference = response.tx.txRef;
                $.ajax({
                    url: '/Movies/updateFlutterwave',
                    data: { reference: response.tx.txRef, flwRef: response.tx.flwRef },
                    dataType: 'json',
                    type: 'POST',
                    success: function (data) {
                        if (data.error == false) {
                            //redirect to confirmation payment page

                            alert("Payment Successful, please check your email for payment confirmation.");
                            window.location.href = "https://ticketplanet.ng/Movies/paymentConfirmationFlw?reference=" + tkReference + "&fltRef=" + flutterRef;
                            //window.location.href = "https://ticketplanet.ng/Movies/paymentConfirmationFlw?reference=" + tkReference + "&fltRef=" + flutterRef;
                            //console.log(response.tx.redirectUrl);
                        } else {

                        }
                    }
                });


            } else {
                // redirect to a failure page.

            }

            x.close(); // use this to close the modal immediately after payment.
        }
    });
}