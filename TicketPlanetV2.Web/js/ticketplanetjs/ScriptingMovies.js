
$(document).ready(function () {
    //$('#loaderbody').show();
    var filmName = $("#MovieName").val();
    var cinemaLocation = $("#CinemaCompanyID").val();
    var filmCode = $("#FilmCode1").val();
    console.log("Film " + filmName);
    console.log("cinemaLoc " + cinemaLocation);
    $.ajax({
        url: '/Movies/GetMovieDays',
        data: { filmName: filmName, cinemaLocation: cinemaLocation},
    dataType: "json",
    type: "POST",

    success: function (data) {
        // alert("hhh")
        $("#MovieDay").html("");
        $("#MovieDay").append($('<option></option>').val(null).html("--Select Movie Day--"));
        $('#preloader').hide();

        //console.log(data);

        if (data.list === 0) {
            alert("No Movie Day available");
        }

        $.each(data.list, function (item, lct)
        {
            //alert(lct.PerformDate)
            //alert(lct.FilmCode)
            $("#MovieDay").append($('<option></option>').val(lct.PerformDate).html(lct.PerformDate))

        })

        //alert(data.FCode);

        $("#FilmCode").val(data.FCode);
    },
    error: function (data) {
        if (data === 0) {
            alert("No Movie time available");
        }
    }

    });

});

$("#CinemaCompanyID").on('change', function () {
    $('#MovieDay').prop('disabled', false);
    $('#MovieDay option:eq(0)').attr('selected', 'selected')

    var filmName = $("#MovieName").val();
    var cinemaLocation = $("#CinemaCompanyID").val();
    console.log("cinema id " + cinemaLocation);

    $("#MovieTime").html("");
    $("#MovieTime").val("");

    //   alert(FromRoute)
    $.ajax({
        url:'/Movies/GetMovieDays',
        data: { filmName: filmName, cinemaLocation: cinemaLocation},
    dataType: "json",
    type: "POST",

    success: function (data) {
        // alert("hhh")
        $("#MovieDay").html("");
        $("#MovieDay").append($('<option></option>').val(null).html("--Select Movie Day--"));
        console.log('data day ' + data.list);
        $.each(data.list, function (item, lct) {
            //alert(lct.PerformDate)
            //alert(lct.FilmCode)
            
            console.log("pda " + lct);
            if (lct.PerformDate !== null) {
                $("#MovieDay").append($('<option></option>').val(lct.PerformDate).html(lct.PerformDate))
            } else {
                alert("No movie Time available");
            }

        })

        //alert(data.FCode);

        $("#FilmCode").val(data.FCode);
    }
});


});

$("#MovieDay").on('change', function () {
    //$("#preloader").delay(350).fadeIn();
    //$("#status").fadeIn();
    $('#loaderbody').show();
    
    $('#MovieTime').prop('disabled', false);
    $('#MovieTime option:eq(0)').attr('selected', 'selected')

    var MovieDay = $("#MovieDay").val();
    var CinemaCompanyID = $("#CinemaCompanyID").val();
    var FilmCode = $("#FilmCode").val();


    //   alert(FromRoute)
    $.ajax({
        url: '/Movies/GetMovieTime',
        data: { MovieDay: MovieDay, CinemaCompanyID: CinemaCompanyID, FilmCode: FilmCode },
    dataType: "json",
    type: "POST",

    success: function (data) {
        // alert("hhh")
        $("#MovieTime").html("");
        $("#MovieTime").append($('<option></option>').val(null).html("Movie Time"));

        //if (data.length == 0) {
        //    alert(data);
        //}

        if (data.location === '0') {
            alert("No Movie time available for today, Please select another day!");
        }
        $.each(data, function (item, lct)
        {
            if (lct.StartTime !== null) {
                $("#MovieTime").append($('<option></option>').val(lct.StartTime).html(lct.StartTime))
            }


        })


        $('#loaderbody').hide();
        //
        //$("#preloader").delay(350).fadeOut();
        //$("#status").fadeOut();

        $("#Amount").html("");

        $("#Amount").val("");
    }
});


});

$("#MovieTime").on('change', function ()
{
    var MovieCategory = $("#MovieCategory").val();
    var NoOfPersons = $("#NoOfPersons").val();
    var CinemaCompanyID = $("#CinemaCompanyID").val();
    var MovieDay = $("#MovieDay").val();
    var FilmCode = $("#FilmCode").val();
    var CouponValue = $("#CouponValue").val();
    var MovieTime = $("#MovieTime").val();

    $('#loaderbody').show();
    //$("#status").fadeIn();
    //$("#preloader").delay(350).fadeIn("slow");
    //var MovieTime = $("#MovieTime").val();
    //console.log(MovieTime);
    $.ajax({
        url: '/Movies/CheckMovieTime',
        data: { movieTime: MovieTime, movieDay: MovieDay },
    dataType: "json",
    type: "POST",
    success: function(data) {
        //console.log(data);
        if (data.nErrorCode === '0') {
                    
            alert("The current Movie time has expired, Please select another time");
            //$("#status").fadeOut();
            //$("#preloader").delay(350).fadeOut("slow");
            $('#loaderbody').hide();
            alertify.error("The current Movie time has expired, Please select another time");
            $("#Amount").val("");
            $("#Amount").html("");
        }
        else {
            if (MovieCategory == null || MovieCategory === "")
            {
                //$("#status").fadeOut();
                //$("#preloader").delay(350).fadeOut("slow");
                $('#loaderbody').hide();
                $(".amountText").show();
                return;
            }


            $.ajax({
                url: '/Movies/GetMovieAmountViaTime',
                data: { MovieCategory: MovieCategory, NoOfPersons: NoOfPersons, CinemaCompanyID: CinemaCompanyID, MovieDay: MovieDay, FilmCode: FilmCode, CouponValue: CouponValue, MovieTime: MovieTime },
            dataType: "json",
            type: "POST",

            success: function (data)
            {
                $("#loaderbody").show();

                if (data.OrigAmount == null)
                {
               

                    if (data.Amount == "0.00") {
                        alert("No Amount Available");
                        $('#loaderbody').hide();
                        //$("#status").fadeOut();
                        //$("#preloader").delay(350).fadeOut("slow");
                        $("#Amount").val(data.Amount);
                        $("#amtCharge").val(data.amtCharge);
                        $(".amountText").show();
                    
                    }
                    else {
                        //$("#status").fadeOut();
                        //$("#preloader").delay(350).fadeOut("slow");
                        $('#loaderbody').hide();
                        $("#Amount").val(data.Amount);
                        $(".amountText").show();
                        $("#amtCharge").val(data.amtCharge);
                    }

                }
                else if(data.OrigAmount != null)
                {
                    $('#loaderbody').hide();
                    //$("#status").fadeOut();
                    //$("#preloader").delay(350).fadeOut("slow");
                    $("#Amount").val(data.Amount);
                    $("#OrigAmount").text(data.OrigAmount);
                    $("#OrigAmount").show();
                    $("#NewAmount").text(data.OrigAmount);
                    $("#OrigAmount").show();
                    $("#CouponValue").val(data.CouponValue);
                    $(".amountText").hide();
                    $("#amtCharge").val(data.amtCharge);
                }
                $('#loaderbody').hide();
            },
            error: function () {
                $('#loaderbody').hide();
                //$("#status").fadeOut();
                //$("#preloader").delay(350).fadeOut("slow");
                $(".amountText").hide();
            }
        });
    }
    $("#loaderbody").hide();
}
});
        
       
});

$("#NoOfPersons").on('change', function () {
    $('#loaderbody').show();
    //$("#status").fadeIn();
    //$("#preloader").delay(350).fadeIn("slow");
    var MovieCategory = $("#MovieCategory").val();
    var NoOfPersons = $("#NoOfPersons").val();
    var CinemaCompanyID = $("#CinemaCompanyID").val();
    var MovieDay = $("#MovieDay").val();
    var FilmCode = $("#FilmCode").val();
    var CouponValue = $("#CouponValue").val();
    var MovieTime = $("#MovieTime").val();
    //console.log('change');
    if (MovieCategory == null || MovieCategory == '')
    {
        $('#loaderbody').hide();
        alertify.error("Please select a Movie Ticket Category");
        return
        //$("#status").fadeOut();
        //$("#preloader").delay(350).fadeOut("slow");
        //$("#loaderbody").hide();
    }
    if (MovieDay == "" || MovieDay == '') {
        $('#loaderbody').hide();
        alertify.error("Please select a Movie Ticket Date");
        return
        //$("#status").fadeOut();
        //$("#preloader").delay(350).fadeOut("slow");
        //$("#loaderbody").hide();
    }
    //console.log(MovieDay);
    if (MovieTime == null || MovieTime == '') {
        $('#loaderbody').hide();
        alertify.error("Please select a Movie Ticket Time");
        return
        //$("#status").fadeOut();
        //$("#preloader").delay(350).fadeOut("slow");
        //$('#loaderbody').hide();
    }

    $.ajax({
        url: '/Movies/GetMovieAmount',
        data: { MovieCategory: MovieCategory, NoOfPersons: NoOfPersons, CinemaCompanyID: CinemaCompanyID, MovieDay: MovieDay, FilmCode: FilmCode, CouponValue: CouponValue, MovieTime: MovieTime },
    dataType: "json",
    type: "POST",

    success: function (data)
    {
        //$('#loaderbody').show();
        if (data.OrigAmount == null)
        {

            $("#Amount").val(data.Amount);
            $('#amtCharge').val(data.amtCharge);
            $("#loaderbody").hide();
            //$("#status").fadeOut();
            //$("#preloader").delay(350).fadeOut("slow");
            $(".amountText").show();
            var x = $("#Amount").val(data.Amount);
            var y = $("#amtCharge").val(data.amtCharge);
            var totalAmounts = parseFloat(x) + parseFloat(y);
            //alert(totalAmounts);
        }
        else if(data.OrigAmount != null)
        {
            $("#Amount").val(data.Amount);
            $("#amtCharge").val(data.amtCharge);
            $("#OrigAmount").text(data.OrigAmount);
            $("#OrigAmount").show();
            $("#NewAmount").text(data.OrigAmount);
            $("#OrigAmount").show();
            //$("#CouponValue").val(data.CouponValue);
            //$("#status").fadeOut();
            $("#loaderbody").hide();
            //$("#preloader").delay(350).fadeOut("slow");
            $(".amountText").hide();
            var x = $("#Amount").val(data.Amount);
            var y = $("#amtCharge").val(data.amtCharge);
            var totalAmounts = parseInt(data.Amount) + parseInt(data.amtCharge);
            //alert(totalAmounts);
        }

        $("#loaderbody").hide();
    }
});


});


$("#MovieCategory").on('change', function () {

    $('#loaderbody').show();
    //$("#status").fadeIn();
    //$("#preloader").delay(350).fadeIn("slow");
    var MovieCategory = $("#MovieCategory").val();
    var NoOfPersons = $("#NoOfPersons").val();
    var CinemaCompanyID = $("#CinemaCompanyID").val();
    var MovieDay = $("#MovieDay").val();
    var FilmCode = $("#FilmCode").val();
    var CouponValue = $("#CouponValue").val();
    var MovieTime = $("#MovieTime").val();
    //   alert(FromRoute)
    console.log(MovieTime + ", " + FilmCode + " , " + MovieCategory);
    console.log(MovieDay);
    if (MovieTime == null || MovieTime == "") {
        //$("#status").fadeOut();
        //$("#preloader").delay(350).fadeOut("slow");
        $('#loaderbody').hide();
        $('.amountText').hide();
    }

    if (MovieDay == "" || MovieDay == "") {
        //$("#status").fadeOut();
        //$("#preloader").delay(350).fadeOut("slow");
        $('#loaderbody').hide();
        $('.amountText').show();
    }


    //console.log("check movie time ");
    $.ajax({
        url: '/Movies/CheckMovieTime',
        data: { movieTime: MovieTime },
        dataType: "json",
        type: "POST",
        success: function (data) {

            if (data.nErrorCode == '0') {
                alert("The current Movie time has expire, Please select another time");
                //$("#status").fadeOut();
                //$("#preloader").delay(350).fadeOut("slow");
                $('#loaderbody').hide();
                alertify.error("The current Movie time has expire, Please select another time");
                $("#Amount").val("");
                $("#Amount").html("");
            }
            else {
                //console.log("check Get Moview Amount");
                $.ajax({

                    url: '/Movies/GetMovieAmount',
                    data: { MovieCategory: MovieCategory, NoOfPersons: NoOfPersons, CinemaCompanyID: CinemaCompanyID, MovieDay: MovieDay, FilmCode: FilmCode, CouponValue: CouponValue, MovieTime: MovieTime },
                    dataType: "json",
                    type: "POST",
                    success: function (data) {
                        //console.log("data " + data);
                        //$("#loaderbody").show();
                        if (data.OrigAmount == null) {

                            if (data.Amount == "0.00") {
                                alert("No Amount Available");
                                $('#loaderbody').hide();
                                //$("#status").fadeOut();
                                //$("#preloader").delay(350).fadeOut("slow");
                                $("#Amount").val(data.Amount);
                                $('#amtCharge').val(data.amtCharge);
                                $(".amountText").show();
                                var x = $("#Amount").val(data.Amount);
                                var y = $("#amtCharge").val(data.amtCharge);
                                //console.log('xxx' + x);

                                var totalAmounts = parseInt(data.Amount) + parseInt(data.amtCharge);
                                //alert(totalAmounts);
                            }
                            else if (data.Amount == null) {
                                alert("No Amount Available");
                                $('#loaderbody').hide();
                                //$("#status").fadeOut();
                                //$("#preloader").delay(350).fadeOut("slow");
                                $("#Amount").val(data.Amount);
                                $("#amtCharge").val(data.amtCharge);
                                $(".amountText").show();
                                var x = $("#Amount").val(data.Amount);
                                var y = $("#amtCharge").val(data.amtCharge);
                                var totalAmounts = parseInt(data.Amount) + parseInt(data.amtCharge);
                               // alert(totalAmounts);
                            }
                            else {
                                $('#loaderbody').hide();
                                //$("#status").fadeOut();
                                //$("#preloader").delay(350).fadeOut("slow");
                                console.log("amou " + data);
                                $("#Amount").val(data.Amount);
                                $("#amtCharge").val(data.amtCharge);
                                $(".amountText").show();
                                var x = $("#Amount").val(data.Amount);
                                var y = $("#amtCharge").val(data.amtCharge);
                                var totalAmounts = parseInt(data.Amount) + parseInt(data.amtCharge);
                               // alert(totalAmounts);
                            }

                        }
                        else if (data.OrigAmount != null) {
                            $("#Amount").val(data.Amount);
                            $("#amtCharge").val(data.amtCharge);
                            $("#OrigAmount").text(data.OrigAmount);
                            $("#OrigAmount").show();
                            $("#NewAmount").text(data.OrigAmount);
                            $("#OrigAmount").show();
                            $("#CouponValue").val(data.CouponValue);
                            $('#loaderbody').hide();
                            //$("#status").fadeOut();
                            //$("#preloader").delay(350).fadeOut("slow");
                            $(".amountText").hide();
                            var x = $("#Amount").val(data.Amount);
                            var y = $("#amtCharge").val(data.amtCharge);
                            var totalAmounts = parseInt(data.Amount) + parseInt(data.amtCharge);
                            //alert(totalAmounts);
                        }
                    }
                });
            }
            $("#loaderbody").hide();
        }
    });


});

