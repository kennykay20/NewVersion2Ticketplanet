function GetCinemaLoactions()
{
    $("#loaderbody").show();
    var cinema = $("#drpCinemaCompany").val();
    //console.log("Another Cinema Val = " + cinema);
    $.ajax
        ({
            url: $('#GetCinemaCompnies').data('request-url'),
            type: 'POST',
            datatype: 'application/json',
            contentType: 'application/json',
            data: JSON.stringify({
                cinema: +cinema
            }),
            
            success: function (data) {
                //console.log("Result", data);
                $("#drpLocation").html("");
                $.each(data, function (item, lct) {
                    // alert(lct.LocationName)
                    $("#drpLocation").append($('<option></option>').val(cinema === "3" ? lct.SiteId:lct.Itbid).html(lct.CinemaName));
                });
               
                $('#OthersFilmHouse').html("");
                GetMovies();
                //$("#loaderbody").hide();
            },
            error: function () {
                alert("Whooaaa! Something went wrong..");
                $("#loaderbody").hide();
            }
        });
} 




$("#drpLocation").on('change', function () {
    //alert("test")
    $("#loaderbody").show();
    $('#OthersFilmHouse').html('');
    var company = $('#drpCinemaCompany').val();
    var location = $('#drpLocation').val();
    var Urls = $('#GetMovies').data('request-url');
    //console.log("Urls =" + Urls);

    //console.log("Locations out = " + location);

  //  alert(location)
    $.ajax({
        //url: $('#GetMovies').data('request-url'),
        url: "/Movies/GetMoviesViaLocation",
        type: 'POST',
        cache: false,
        dataType: 'html',
        data: { company: company, location: location },
        success: function (data, textStatus, XMLHttpRequest) {
            if (data !== null)
            {
             
                $('#FilmHouse').show();
                $('#OthersFilmHouse').html('');
                $('#OthersFilmHouse').html(data);
                //console.log("Movies1 " + data);
                $("#loaderbody").hide();
            }

           
        },

        error: function (xhr, ajaxOptions, thrownError) {
            $('#screenDiv').hide();
            $("#loaderbody").hide();
        },


    });

});


function GetMovies()
{
    var company = $('#drpCinemaCompany').val();
    var location = $('#drpLocation').val();
    //alert(company)
    //alert(location)
    $("#loaderbody").show();
    $.ajax({
        url: $('#GetMovies').data('request-url'),
        type: 'POST',
        cache: false,
        dataType: 'html',
        data: { company: company, location: location },
        success: function (data, textStatus, XMLHttpRequest) {
            if (data !== null) {
                //console.log(data);
                $('#FilmHouse').show();
                $('#OthersFilmHouse').html('');
                //$("#loaderbody").show();
                $('#OthersFilmHouse').html(data);
                $("#loaderbody").hide();
            }


        },

        error: function (xhr, ajaxOptions, thrownError) {
            $('#screenDiv').hide();
        },


    });


}


$(".btnEventIndexGrid").on('click', function () {
    //alert("hhh")
    var TicketType = $(this).data('key');
    var ref = "";
    if (TicketType != null) {
       
        window.location = $('#gotoEventTicket').data('request-url') + '?TicketType=' + TicketType + '&referalId=' + ref;

    }

});

function validateEmail(email) {

    //const re = "\\w+([-+.']\\w+)*@@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
    const re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

$(document).ready(function () {


    $(".btnSubmitComment").on('click', function (e) {
        e.preventDefault();

        

        if ($("#name").val() === "" || $('#comment').val() === "" ||
                    $('#email').val() === "") {

            alertify.error("One or Two Fields is Empty");
            return;
        }
        var name = $("#name").val();
        var comment = $("#comment").val();
        var email = $("#email").val();

        if (validateEmail(email)) {
            $.ajax({
                type: "POST",
                url: "/Home/Contact",
                data: { name: name, emailfrom: email, comment: comment },
                dataType: 'json',
                success: function (data, textStatus, XMLHttpRequest) {
                    if (data.error == false) {

                        alert(data.message);
                        alertify.success(data.message);
                        //console.log(data.message);
                        $("#name").val('');
                        $("#comment").val('');
                        $("#email").val('');
                        //console.log(data);
                    }
                },
            });
        }
        else {
            alert(email + " is not valid");
            alertify.error("Please enter a valid email address");
        }
        
    });
});




