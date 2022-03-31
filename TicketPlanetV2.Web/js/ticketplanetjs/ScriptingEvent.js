$("#btnVoucher").on('click', function () {
    $('#btnVoucher').attr('disabled', true);
    $('#loadVoucherCode').show()
    var VoucherCode = $("#VoucherCode").val();
    if (VoucherCode == "") {
        alertify.error("Kindly input a Voucher Code", "", 0);
        $('#btnVoucher').attr('disabled', false);
        return
    }
    var Amount = $("#Amount").val();

    //  alert(Amount);

    $.ajax({
        url: "/Events/VerifyCoupon",
        data: { coupon: VoucherCode },
        dataType: "json",
        type: "POST",
        success: function (data) {
            $('#btnVoucher').attr('disabled', false);
            $('#loadVoucherCode').hide()
            //  alert(data.nErrorCode)
            if (data != null) {
                if (data.nErrorCode == 1) {
                    if (Amount != "") {
                        var amt = Number(Amount.replace(/[^0-9\.-]+/g, ""));
                        var tranAmount = (data.CouponValue / 100) * amt;
                        $("#Amount").val(tranAmount);
                    }

                    $('#searchResults1').hide()
                    $('#searchResults').delay(800).slideDown(500);
                    $('#searchResults').html(data.sErrorText)
                    alertify.log(data.sErrorText, "", 0);

                    $("#lblCoupon").text(data.sErrorText);
                    $("#lblCoupon").show();
                    $("#nErrorCode").val(data.nErrorCode);

                    $("#CouponValue").val(data.CouponValue);
                    $("#CouponID").val(data.CouponId);

                }
                else {
                    alertify.error(data.sErrorText, "", 0);

                    $("#lblCoupon").hide();
                    ("#nErrorCode").val(data.nErrorCode);
                    $("#IsCoupon").val("N");
                }
            }
            else {
                $("#lblCoupon").hide();

            }
        }
    });//end ajax calling
});


$("#TicketCategory").on('change', function () {
    
    //$("#preloader").delay(350).fadeIn();
    //$("#status").fadeIn();
    $('#loaderbody').show();
    var TicketCategory = $("#TicketCategory").val();
    var eventName = $("#eventName").val();
    var NoOfPersons = $("#NoOfPersons").val();
    var TicketCategoryName = $("#TicketCategory>option:selected").text();
    //var NoOfPersons = $("#NoOfPersons").val();
    var CouponValue = $("#CouponValue").val();
    //   alert(FromRoute)
    //console.log(TicketCategoryName);
    //console.log(TicketCategory);
    $.ajax({
        url: "/Events/GetEventAmount",
        data: { TicketCategoryID: TicketCategory, TicketCategoryName: TicketCategoryName, NoOfPersons: NoOfPersons, CouponValue: CouponValue, EventName: eventName },
        dataType: "json",
        type: "POST",

        success: function (data) {

            if (data.nErrorCode == 0) {
                if (data.OrigAmount == null) {
                    $("#Amount").val(data.Amount);
                    //$("#status").fadeOut();
                    //$("#preloader").delay(350).fadeOut("slow");
                }
                else if (data.OrigAmount != null) {
                    $("#Amount").val(data.Amount);
                    $("#OrigAmount").text("₦" + data.OrigAmount);
                    $("#OrigAmount").show();
                    $("#NewAmount").text(data.OrigAmount);
                    $("#OrigAmount").show();
                    //$("#status").fadeOut();
                    //$("#preloader").delay(350).fadeOut("slow");
                }
            }
            else {
                //$("#status").fadeOut();
                //$("#preloader").delay(350).fadeOut("slow");
                alert("No Amount Available");
                $("#Amount").val(data.Amount);
            }

            $('#loaderbody').hide();


        }
    });


});

$("#NoOfPersons").on('change', function () {
    $('#loaderbody').show();
    //$("#preloader").delay(350).fadeIn();
    //$("#status").fadeIn();
    var TicketCategory = $("#TicketCategory").val();
    var eventName = $("#eventName").val();
    var NoOfPersons = $("#NoOfPersons").val();
    var TicketCategoryName = $("#TicketCategory>option:selected").text();
    //var NoOfPersons = $("#NoOfPersons").val();
    var CouponValue = $("#CouponValue").val();
    //   alert(FromRoute)
    $.ajax({
        url: "/Events/GetEventAmount",
        data: { TicketCategoryID: TicketCategory, TicketCategoryName: TicketCategoryName, NoOfPersons: NoOfPersons, CouponValue: CouponValue, EventName: eventName },
        dataType: "json",
        type: "POST",

        success: function (data) {

            if (data.nErrorCode == 0) {
                if (data.OrigAmount == null) {
                    $("#Amount").val(data.Amount);
                    //alert(data.discountPercentage);
                    //$('#loaderbody').hide();
                }
                else if (data.OrigAmount != null) {
                    $("#Amount").val(data.Amount);
                    $("#OrigAmount").text("₦" + data.OrigAmount);
                    $("#OrigAmount").show();
                    $("#NewAmount").text(data.OrigAmount);
                    $("#OrigAmount").show();
                    //$('#loaderbody').hide();
                }
            }
            else {
                $("#Amount").val(data.Amount);
                //$('#loaderbody').hide();
                alert("No Amount Available");
            }

            $('#loaderbody').hide();
            //$("#preloader").delay(350).fadeOut();
            //$("#status").fadeOut();
        }
    });


});
