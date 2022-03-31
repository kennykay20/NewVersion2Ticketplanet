$(function () {



    function InitFlwTransaction(data) {
        return $.ajax({
            type: "POST",
            url: "/Events/FlutterwaveEventPayment/",
            data: data,
            dataType: 'json',
        contentType: 'application/json;charset=utf-8'
    });
}

    $("#IntializeEventFlwPayment").click(function (e) {


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
                    $('#NoOfPersons').val() === "" || $('#currency').val() === "") {

                    alertify.error("One or Two Compulsory Fields is Empty");
                    return;
                }
            }

            $("#preloader").delay(350).fadeIn();
            $("#status").fadeIn();

            //$("#status").fadeOut();
            //$("#preloader").delay(350).fadeOut("slow");
            e.preventDefault();

            var currency = $("#currency").val();
            //console.log(currency);
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
                ReferalId: $("#referalId").val()

            });


            $.when(InitFlwTransaction(data)).then(function (response) {
                if (response.error == false) {
                    $("#status").fadeOut();
                    $("#preloader").delay(350).fadeOut("slow");
                    //console.log(response.result);
                    //console.log(('#amtCharge').val());
                    //console.log(totalAmounts);
                    //console.log(response.result);
                    payWithRave(response.result.publicKey, response.result, currency);
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

function payWithRave(pbKeys, data, currency) {
    
       
    var txtRef = data.Reference;
    

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
            //var fltRef = response.tx.txRef; // collect txRef returned and pass to a server page to complete status check.
            //console.log("This is the response returned after a charge", response);
            //console.log(response);
            if (response.tx.chargeResponseCode == "00")
            {
                    
                var flutterRef = response.tx.flwRef;
                var tkReference = response.tx.txRef;

                $.ajax({
                        
                    url: "/Events/updateFlutterPayment",
                    data: { reference: response.tx.txRef, flwRef: response.tx.flwRef },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    if (data.error == false) {
                        //redirect to confirmation payment page
                        alert("Payment Successful, please check your email for payment confirmation.");
                                
                                
                        //console.log(response.tx.redirectUrl);
                        window.location.href = "http://localhost:2070/Events/paymentConfirmationFlw?reference=" + response.tx.txRef + "&flwRef=" + response.tx.flwRef;
                                
                    } else {
                        //alert("Error to update table for SUCCESSFUL");
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