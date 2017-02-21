function refrashPage() {
    deleteMap();
    loadPreviewImages();
    loadFullsizeImage();
}

function loadPreviewImages() {

    $('ul').empty();
    $.get("/Home/IsImageCollectionEmpty",
              function (isCollectionEmpty) {
                  if (isCollectionEmpty == 'False') {
                      $('ul').load('/Home/PreviewSection');
                  }
              });
}

function loadFullsizeImage() {

    $('#fullsizeSection').empty();

    $.get("/Home/IsImageCollectionEmpty",
            function (isCollectionEmpty) {
                if (isCollectionEmpty == 'False') {
                    $('#fullsizeSection').load('/Home/FullsizeSection?id=' + $("ul li:first-child img:first-child")[0].id);
                } else {
                    commentHidden();
                }
            });
}

function loadLastImage() {

    $.get("/Home/GetLastImageId",
   function (result) {
       var lastImageId = result;

       $.post("/Home/GetLastImageByteData", { idStr: lastImageId }).done(
        function (lastImageByteData) {
            $("ul").prepend("<li>" + "<img class='previewImg' id='" + lastImageId +
                "' style='width:180px; height:150px;' src=\"data:image/jpeg;base64," +
                lastImageByteData + "\" />") + "</li>";

            hangClickHandlers();
        });
   });
}

function hangClickHandlers() {
    $('.previewImg').click(function (e) {
        e.preventDefault();
        var id = e.currentTarget.id
        if (id != undefined) {
            $('#fullsizeSection').load('/Home/FullsizeSection?id=' + id);
            e.stopPropagation();
        }
        else {
            alert("undefined id")
        }
    });
}

function uploadImage() {
    var files = document.getElementById('uploadFile').files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({
                type: "POST",
                url: '/Home/Upload',
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    alert(result);
                    loadLastImage();
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);

                }
            });
        }
    }
}

function getMap(long, lat) {

    google.maps.visualRefresh = true;

    var center = new google.maps.LatLng(lat, long);

    var mapOptions = {
        zoom: 15,
        center: center,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };

    var map = new google.maps.Map(document.getElementById("gmapsSection"), mapOptions);

    var myLatlng = new google.maps.LatLng(lat, long);

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: 'Position'
    });
    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');

}

function deleteMap() {
    $("#gmapsSection").empty();
}

function deleteImages() {

    $.get("/Home/IsImageCollectionEmpty",
            function (isCollectionEmpty) {
                if (isCollectionEmpty == 'False') {
                    if (confirm("Are you sure you want to delete all images?")) {
                        $.post('/Home/DeleteAllImages', function () {
                            refrashPage()
                            alert("All images are deleted")
                        });
                    }     
                } 
            });
}


function insertComment() {
    var comment;
    if ($("#fImg").attr('name') != undefined) {
        $.post('/Home/GetImageComment?id=' + $("#fImg").attr('name'), function (comment) {
            $("#comment").text(comment);
        });
    }
}

function saveComment() {

    var newComment = $("#commentTextBox").val();
    var currentImgId = $("#fImg").attr('name');

    if (newComment != undefined && currentImgId != undefined) {

        $.post('/Home/SaveComment',
            {
                param1: currentImgId,
                param2: newComment
            }, function () {
                insertComment();
            });

        $("#commentTextBox").addClass("hidden");
        $("#comment").removeClass("hidden");
        $("#comment").val("");
        $("#commentSave").addClass("hidden");
        $("#commentEdit").removeClass("hidden");
    } else {
        alert('Please make sure to input correct comment')
    }
}


function editComment() {
    $("#comment").addClass("hidden");
    $("#commentTextBox").removeClass("hidden");
    $("#commentEdit").addClass("hidden");
    $("#commentSave").removeClass("hidden");
}

function commentHidden() {
    $('#commentSection').addClass('hidden');
}
function commentShow() {
    $('#commentSection').removeClass('hidden');
}

function validateInputFile() {

    $('#uploadFile').change(function () {
        var filesExt = ['jpg, jpeg'];
        var maxSize = 1024 * 1024 * 10;
        var flagExt = false;
        var flagSize = false;

        var parts = $(this).val().split('.');
        if (filesExt.join().search(parts[parts.length - 1]) != -1) {
            flagExt = true;
        } else {
            flagExt = false;
        }
        if ($(this)[0].files[0].size <= maxSize) {
            flagSize = true;
        } else {
            flagSize = false;
        }

        if (!flagExt) {
            alert('Incorrect file format. Please download the pictures in the format jpg/jpeg');
            $(this).val(undefined);
        } else {
            if (!flagSize) {
                alert('File size exceeds the allowable(<10Mb)');
                $(this).val(undefined);
            }
        }
    });
}