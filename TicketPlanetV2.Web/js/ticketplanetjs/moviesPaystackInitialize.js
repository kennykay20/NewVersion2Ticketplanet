$(function () {



    function InitTransaction(data) {
        return $.ajax({
            type: "POST",
            url: "/Movies/InitializeMoviePayment",
            data: data,
            dataType: 'json',
        contentType: 'application/json;charset=utf-8'
    });
}

           $("#IntializePayment").click(function (e) {


               if ($("#Fullname").val() === "" || $('#phoneNo').val() === "" ||
                        $('#email').val() === "" ||
                        $('#NoOfPersons').val() === "" ||
                        $('#MovieCategory').val() === "" ||
                        $('#Amount').val() === "" || $('#MovieDay').val() === "" || $('#MovieTime').val() === "")
               {

                   alertify.error("One or Two Compulsory Fields is Empty");
                   return;
               }

               //$("#preloader").show();
               $("#status").fadeIn();
               $("#preloader").delay(350).fadeIn("slow");
               e.preventDefault();

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



               $.when(InitTransaction(data)).then(function (response) {


                   if (response.error == false) {
                       $("#status").fadeOut();
                       $("#preloader").delay(350).fadeOut("slow");
                       //console.log(response.result);
                       window.location.href = response.result.data.authorization_url;
                        
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
