var chosenI = 0;
var chosenId = 0;
var stat;
var stat2;
var pId;
var isEnable;
var isDeleted;
var mId;
var tId;

var etType;
var cType;
var curr;
var partner;
var reseller;
var merchant;
var branch;
var pos;
var eAction;
var eStart;
var eEnd;

var cid;

var merchantId = 0;

var midName;
var cardTypeId = 0;

var errorMsg;

function readyForAction(obj) {
    chosenI = obj.getAttribute("data-value");

    $("#dvActionKeys").show();
}

function refreshAndroidAppVersion() {
    var id;
    $('#androidVerTbl').dataTable().fnDestroy();

    $('#androidVerTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "AndroidAppVersionId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    var id = data;
                    return renderMe;
                }
            },
            { data: "AppName" },
            { data: "PackageName" },
            { data: "VersionName" },
            { data: "VersionCode" },
            { data: "VersionBuild" },
            { data: "Description" },
            {
                data: "Status", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Data, would you like to <a href='/AndroidMpos/AddInfo'>register</a>?"
        },
        "ajax": {
            url: rootDir + 'Ajax/AndroidAppVersionList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function updateStatus(obj) {
    var chosen = obj.getAttribute("value");
    chosenId = obj.getAttribute("id");
    var status;

    if (chosen == "Deactivate")
        stat = "false";
    else if (chosen == "Activate")
        stat = "true";

    $.ajax({
        url: rootDir + "Switches/UpdateSwitchStatus",
        type: 'POST',
        data: { "status": stat, "id": chosenId.substring(0, 2) },
        success: function (data) {

            if (data.IsActive == true) {
                enable = "Deactivate";
                status = "Activated";
            } else {
                enable = "Activate";
                status = "Deactivated";
            }
            obj.setAttribute("value", enable);
            document.getElementById(chosenId.substring(0, 2) + "td").textContent = status;
        }
    });
}

function updatePartnerSwitch(obj) {
    var id = obj.getAttribute("id");
    var enable;
    var status;
    isEnable = document.getElementById(id + "enable").textContent;

    if (isEnable == "Yes") {
        isEnable = false;
    } else {
        isEnable = true;
    }
    $.ajax({
        url: "EnableSwitchforPartner",
        type: 'POST',
        data: { "id": id, "pId": pId, "isEnabled": isEnable },
        success: function (data) {
            if (data.IsEnabled == true) {
                enable = "Disable";
                status = "Yes";
            } else {
                enable = "Enable";
                status = "No";
            }
            obj.setAttribute("value", enable);
            document.getElementById(id + "enable").textContent = status;
        }
    });
}

function updateUserStatus(obj) {
    var id = obj.getAttribute("id");
    var status;
    var status2;
    isEnable = document.getElementById(id).textContent;

    if (isEnable == "Deactivate") {
        isEnable = false;
    } else if (isEnable == "Activate") {
        isEnable = true;
    } else if (isEnable == true) {
        isEnable = true;
    } else if (isEnable == false) {
        isEnable = false;
    }

    $.ajax({
        url: rootDir + "Users/UpdateUserStatus",
        type: 'POST',
        data: { "aId": id, "isEnable": isEnable },
        success: function (data) {
            if (data.IsActive == true) {
                status = "Deactivate";
                status2 = "User Active";
            } else {
                status = "Activate";
                status2 = "User Account Deactivated";
            }
            document.getElementById(id).textContent = status;
            document.getElementById(id + "enable").textContent = status2;
        }
    });
}

function updateMerchantStatus(obj) {
    var id = obj.getAttribute("merchantid");
    var status = obj.getAttribute("value");

    var stat;

    if (status == "Deactivate") {
        stat = false;
    } else if (status == "Activate") {
        stat = true;
    }

    $.ajax({
        url: rootDir + "Merchants/UpdateMerchantStatus",
        type: 'POST',
        data: { "mId": id, "status": stat },
        success: function (data) {
            if (data.IsActive == true) {
                status = "Deactivate";
                $('#mStatus' + id + '').removeClass();
                $('#mStatus' + id + '').addClass("btn btn-success");
            } else {
                status = "Activate";
                $('#mStatus' + id + '').removeClass();
                $('#mStatus' + id + '').addClass("btn btn-default");
            }

            document.getElementById('mStatus' + id).value = status;
        }
    });
}

function updateMerchantBranchStatus(obj) {
    var id = obj.getAttribute("merchantbranchid");
    var status = obj.getAttribute("value");

    var stat;

    if (status == "Deactivate") {
        stat = false;
    } else if (status == "Activate") {
        stat = true;
    }

    $.ajax({
        url: rootDir + "MerchantBranches/UpdateMerchantBranchStatus",
        type: 'POST',
        data: { "bId": id, "status": stat },
        success: function (data) {
            if (data.IsActive == true) {
                status = "Deactivate";
                $('#mbStatus' + id + '').removeClass();
                $('#mbStatus' + id + '').addClass("btn btn-success");
            } else {
                status = "Activate";
                $('#mbStatus' + id + '').removeClass();
                $('#mbStatus' + id + '').addClass("btn btn-default");
            }

            document.getElementById('mbStatus' + id).value = status;
        }
    });
}

function updateMerchantBranchPOSStatus(obj) {
    var id = obj.getAttribute("merchantbranchposid");
    var status = obj.getAttribute("value");

    var stat;

    if (status == "Deactivate") {
        stat = false;
    } else if (status == "Activate") {
        stat = true;
    }

    $.ajax({
        url: rootDir + "MerchantBranchPOSs/UpdateMerchantBranchPOSStatus",
        type: 'POST',
        data: { "pId": id, "status": stat },
        success: function (data) {
            if (data.IsActive == true) {
                status = "Deactivate";
                $('#mbpStatus' + id + '').removeClass();
                $('#mbpStatus' + id + '').addClass("btn btn-success");
            } else {
                status = "Activate";
                $('#mbpStatus' + id + '').removeClass();
                $('#mbpStatus' + id + '').addClass("btn btn-default");
            }

            document.getElementById('mbpStatus' + id).value = status;
        }
    });
}

function updateUserAvailability(obj) {
    var id = obj.getAttribute("id");

    var aId = id.replace("available", "");

    $.ajax({
        url: rootDir + "Users/UpdateUserAvailability",
        type: 'POST',
        data: { "aId": aId },
        success: function (data) {
            document.getElementById(id).disabled = true;

            $("#msg").text("User has been unlocked");
            $("#message-box-success").show();
            $("#message-box-success").fadeOut(1500);
        }
    });
}
function updateEmailServer(obj) {
    var id = obj.getAttribute("id");
    var enable = obj.getAttribute("enable");
    var status;
    isEnable = document.getElementById(id).textContent;
    isDeleted = document.getElementById(id).textContent;

    var strId = id;
    if (id == strId) {
        id = id.substring(0, 1);
    }

    if (isEnable == "Delete") {
        isEnable = enable;

        if (isEnable == "Deactivate") {
            isEnable = true;
        } else if (isEnable == "Activate") {
            isEnable = false;
        } else if (isEnable == true) {
            isEnable = false;
        } else if (isEnable == false) {
            isEnable = true;
        }
    } else {
        if (isEnable == "Deactivate") {
            isEnable = false;
        } else if (isEnable == "Activate") {
            isEnable = true;
        } else if (isEnable == true) {
            isEnable = true;
        } else if (isEnable == false) {
            isEnable = false;
        }
    }

    if (isDeleted == "Delete") {
        isDeleted = true;
    } else {
        isDeleted = false;
    }

    $.ajax({
        url: "UpdateEmailServer",
        type: 'POST',
        data: { "id": id, "pId": pId, "isActive": isEnable, "isDeleted": isDeleted },
        success: function (data) {
            if (data.IsActive == true) {
                status = "Deactivate";
            } else {
                status = "Activate";
            }
            obj.setAttribute("value", status);
            refreshEmailServerTable(pId);
        }
    });
}

function registerRequestedMerchant(obj) {
    if ($("#ddlresellers").val() > 0) {
        var rmId = obj.getAttribute("data-value");

        $.ajax({
            url: rootDir + "Merchants/RegisterRequestedMerchants",
            type: 'POST',
            data: { "rId": $("#ResellerId").val(), "rmId": rmId },
            success: function (data) {

                merchantId = data.data.MerchantId;

                document.getElementById('CurrencyId').value = 1;
                document.getElementById('CardTypeId').value = data.data2.CardTypeId;
                document.getElementById('Param_1').value = data.data2.AccessCode;
                document.getElementById('Param_2').value = data.data2.MID;
                document.getElementById('Param_3').value = data.data2.SecureHash;
                document.getElementById('Param_4').value = data.data2.Username;
                document.getElementById('Param_5').value = data.data2.Password;

                if (data.error != undefined) {
                    $("#errorMessage").text("• " + data.error);
                }

                $("#errMsg").text("Merchant successfully created.");
                $("#message-box-success").show();
                $("#message-box-success").fadeOut(1500);
            }
        });

        $(".tabs").show();
        $(".1st").hide();
    } else {
        $("#errorMsg").text("Please choose a reseller before Activating the Merchant.");
        $("#message-box-danger").show();
    }
}

function onClickTransaction(obj) {
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var tType = obj.getAttribute("data-value");
    var currency = obj.getAttribute("data-currency");
    var cardTypeId = obj.getAttribute("data-cardtype");
    var p = document.getElementById('ddlpartners').value;
    var r = document.getElementById('ddlresellers').value;
    var m = document.getElementById('ddlmerchants').value;
    var b = document.getElementById('ddlbranches').value;
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;

    etType = tType;
    cType = cardTypeId;
    curr = currency;
    partner = p;
    reseller = r;
    merchant = m;
    branch = b;
    pos = mp;
    eAction = action;

    if (start == "" && end == "") {
        start = "1/1/0001";
        end = "1/1/0001";
    } else if (start == "") {
        start = "1/1/0001";
    } else if (end == "") {
        end = "1/1/0001";
    }

    eStart = start;
    eEnd = end;

    $("#tTbl1").show();
    $("#tTbl2").hide();

    refreshTransactionTable(p, r, m, mp, b, cardTypeId, tType, action, currency, start, end);
    $(".1st").hide();
    $(".2nd").show();

    $("html, body").animate({ scrollTop: 0 }, 250);
}

function onClickDebitTransaction(obj) {
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var tType = obj.getAttribute("data-value");
    var currency = obj.getAttribute("data-currency");
    var p = document.getElementById('ddlpartners').value;
    var r = document.getElementById('ddlresellers').value;
    var m = document.getElementById('ddlmerchants').value;
    var b = document.getElementById('ddlbranches').value;
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;

    etType = tType;
    curr = currency;
    partner = p;
    reseller = r;
    merchant = m;
    branch = b;
    pos = mp;
    eAction = action;

    if (start == "" && end == "") {
        start = "1/1/0001";
        end = "1/1/0001";
    } else if (start == "") {
        start = "1/1/0001";
    } else if (end == "") {
        end = "1/1/0001";
    }

    eStart = start;
    eEnd = end;

    $("#tTbl1").hide();
    $("#tTbl2").show();
    refreshDebitTransactionTable(p, r, m, mp, b, tType, action, currency, start, end);
    $(".1st").hide();
    $(".2nd").show();


    $("html, body").animate({ scrollTop: 0 }, 250);
}

function removeMids(obj) {
    chosenId = obj.getAttribute("data-value");
    midId = obj.getAttribute("midid");
    $.ajax({
        url: "RemoveAssignedMid",
        type: 'POST',
        data: { "id": chosenId, "midId": midId },
        success: function (data) {
            refreshPOSAssignedMIDsTable(posId);
        }
    });
}

function callMap(lat, long) {

    $('.3rdMap').show();

    var geocoder;
    var map;
    var infowindow = new google.maps.InfoWindow();
    var marker;
    function initialize() {
        geocoder = new google.maps.Geocoder();
        var mapProp = {
            center: new google.maps.LatLng(51.508742, -0.120850),
            zoom: 2,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    }
    google.maps.event.addDomListener(window, 'load', initialize);

    initialize();

    var latlng = new google.maps.LatLng(lat, long);
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[1]) {

                map.setZoom(11);
                marker = new google.maps.Marker({
                    position: latlng,
                    map: map
                });
                infowindow.setContent(results[1].formatted_address);
                infowindow.open(map, marker);

                $('html, body').animate({
                    scrollTop: $(".3rdMap").offset().top
                }, 500);

            } else {
                alert('No results found');
            }
        } else {
            alert('Location Unavailable');
            $('.3rdMap').hide();
        }
    });
}

function sendActivationCode(posId, actCode) {
    var src = rootDir + "img/filetree/spinner.gif";
    $.blockUI({ message: '<h4><img src="' + src + '"> Sending Activation Code ...</h4>' });
    block();

    var mId;

    try {
        mId = $("#ddlmerchants").val();
    } catch (error) {
        mId = 0;
    }

    $.ajax({
        url: rootDir + "MerchantBranchPOSs/SendActivationCode",
        type: 'POST',
        data: { "posId": posId, "actCode": actCode, "mId": mId },
        success: function (data) {

            if (data == "activated") {
                $(document).ajaxStop($.unblockUI);
                $("#err").text("Sending Failed. Activation Code already used.");
                $("#message-box-danger").show();
                $("#message-box-danger").fadeOut(1500);
            } else {
                $(document).ajaxStop($.unblockUI);
                $("#errMsg").text("Activation Code has been sent.");
                $("#message-box-success").show();
                $("#message-box-success").fadeOut(1500);
            }

        },
        error: function (data) {
            $(document).ajaxStop($.unblockUI);
            $("#err").text("Sending Failed. Please check your network connection.");
            $("#message-box-danger").show();
            $("#message-box-danger").fadeOut(1500);
        }
    });
}

function redirectAction(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "/" + chosenI;
}

function redirectActionwithId(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI;
}

function redirectActionwithEncId(obj) {
    if (chosenI != '')
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI;
}

function redirectActionwithmId(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?mId=" + chosenI;
}

function readyForActionAssignMid(obj) {
    chosenI = obj.getAttribute("data-value");
    cid = obj.getAttribute('cid');
    $("#dvActionKeys").show();
}

function redirectActionMidAssign(obj) {
    var mId = document.getElementById('ddlmerchants').value;

    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI + "&" + "mId=" + mId + "&" + "cId=" + cid;
}

function redirectActionEncMidAssign(obj) {
    var mId = document.getElementById('ddlmerchants').value;

    if (chosenI != '')
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI + "&" + "mId=" + mId + "&" + "cId=" + cid;
}

function redirectActionMidRemove(obj) {
    var mId = document.getElementById('ddlmerchants').value;
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI + "&" + "mId=" + mId;
}

function redirectActions(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link");
    localStorage.setItem("chosenId", chosenI);
}

$('#emailserverTbl').on('click', 'button', function (e) {
    var tr = $(this).closest('tr');
    tr.slideUp(300, function () {
        tr.remove();
    });
})

function refreshAddMerchantBulkBoarding(ResellerId) {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {

                data: "MerchantId", header: { content: "masterCheckbox", css: "center" }, "render": function (data, type, full, meta) {
                    var renderMe = "<input type='checkbox' id='chkMerchantId' name='pickMeUp' value='" + data + "'/>";
                    return renderMe;
                },
                "orderable": false
            },
            { data: "MerchantName" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Partner: <b>" + full["PartnerName"]
                        + "</b><br />"
                        + "Reseller: <b>" + full["ResellerName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'Ajax/GetMerchantMidsTobeAdded',
            data: { "id": ResellerId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshViewAddBulkData(MerchantIds) {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantId",
            },
            { data: "MerchantName" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Partner: <b>" + full["PartnerName"]
                        + "</b><br />"
                        + "Reseller: <b>" + full["ResellerName"] + "</b>";
                    return renderMe;
                }
            },
            { data: "CreditCard" },
            { data: "DebitCard" },
            { data: "Command" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'Ajax/ViewAddBulkData',
            data: { "ids": MerchantIds },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshPartnersTable() {
    //$('#partnersTbl').DataTable().fnDestroy();

    $('#partnersTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "PartnerId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            {
                data: "CompanyName"
            },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            {
                data: "ParentPartner.CompanyName", "render": function (data, type, full, meta) {
                    if (data == "-")
                        data = "";
                    return data;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Partners yet, would you like to <a href='/Partners/Registration'>register</a> a new partner?"
        },
        "ajax": {
            url: rootDir + 'Ajax/PartnerList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshResellerTable(partnerId) {

    var strNorow;
    var tData;

    if (partnerId == 0) {
        strNorow = "Select a Partner to View Resellers";
    } else {
        strNorow = "No Resellers yet, would you like to <a href='/Resellers/Registration'>register</a> a new reseller?";
    }

    $('#resellersTbl').dataTable().fnDestroy();

    $('#resellersTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "ResellerId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "ResellerName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            { data: "Partner.CompanyName" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": strNorow
        },
        "ajax": {
            url: rootDir + 'Ajax/ResellerList',
            type: "POST",
            data: { "pId": partnerId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantsTable() {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "MerchantName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Partner: <b>" + full["PartnerName"]
                        + "</b><br />"
                        + "Reseller: <b>" + full["ResellerName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status;

                    if (data == 1) {
                        status = "Inactivate";
                    } else {
                        status = "Active";
                    }

                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants yet, would you like to <a href='/Merchants/Registration'>register</a> a new merchant?"
        },
        "ajax": {
            url: rootDir + 'Ajax/MerchantList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantsFeatureTable(rId) {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "MerchantName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Partner: <b>" + full["PartnerName"]
                        + "</b><br />"
                        + "Reseller: <b>" + full["ResellerName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants found"
        },
        "ajax": {
            url: rootDir + 'Ajax/GetMerchantList',
            data: { "rId": rId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantsTableOnDropdownChange(rId) {
    var mId;
    var strNorow;

    if (rId == 0) {
        strNorow = "Select a Reseller";
    } else {
        strNorow = "No Merchants yet, would you like to <a href='/Merchants/Registration'>register</a> a new merchant?"
    }

    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantId", "render": function (data, type, full, meta) {
                    mId = data;
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "MerchantName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Partner: <b>" + full["PartnerName"]
                        + "</b><br />"
                        + "Reseller: <b>" + full["ResellerName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status;

                    if (data == 1) {
                        status = "Deactivate";
                    } else {
                        status = "Activate";
                    }

                    if (status == "Deactivate") {
                        var link = "<input type='button' style='width:100px;' id='mStatus" + mId + "' class='btn btn-success' merchantid='" + mId + "' value='" + status + "' onclick='updateMerchantStatus(this);'>";
                    } else if (status == "Activate") {
                        var link = "<input type='button' style='width:100px;' id='mStatus" + mId + "' class='btn btn-default' merchantid='" + mId + "' value='" + status + "' onclick='updateMerchantStatus(this);'>";
                    }

                    return link;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": strNorow
        },
        "ajax": {
            url: rootDir + 'Ajax/GetMerchantList',
            type: "POST",
            data: { "rId": rId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshBranchTable(mId) {
    var bId;
    $('#BranchesTbl').dataTable().fnDestroy();
    $('#BranchesTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantBranchId", "render": function (data, type, full, meta) {
                    bId = data;
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "BranchName" },
            { data: "PrimaryContactNumber" },
            { data: "POSs" },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Reseller: <b>" + full["ResellerName"]
                        + "</b><br />"
                        + "Merchant: <b>" + full["MerchantName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status;

                    if (data == 1) {
                        status = "Deactivate";
                    } else {
                        status = "Activate";
                    }

                    if (status == "Deactivate") {
                        var link = "<input type='button' style='width:100px;' id='mbStatus" + bId + "' class='btn btn-success' merchantbranchid='" + bId + "' value='" + status + "' onclick='updateMerchantBranchStatus(this);'>";
                    } else if (status == "Activate") {
                        var link = "<input type='button' style='width:100px;' id='mbStatus" + bId + "' class='btn btn-default' merchantbranchid='" + bId + "' value='" + status + "' onclick='updateMerchantBranchStatus(this);'>";
                    }

                    return link;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Merchant Branch Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/GetBranchesByMerchant',
            type: "POST",
            data: { 'mId': mId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshPOSTable(BranchId) {

    var posId;
    //$('#POSTbl').dataTable().fnDestroy();
    $('#POSTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "POSId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    posId = data;
                    return renderMe;
                }
            },
            {
                data: "POSName", "render": function (data, type, full, meta) {
                    posName = data;
                    return data;
                }
            },
            {
                data: "ActivationCode", "render": function (data, type, full, meta) {
                    var render = "<b>" + data + "<b>" + "<br />" +
                        "<input type='button' class='btn btn-default' value='Email Activation Code' onclick='sendActivationCode(" + posId + "," + "\"" + data + "\")' />";
                    return render;
                }
            },
            {
                data: "null", "render": function (data, type, full, meta) {
                    var renderMe = "Reseller: <b>" + full["ResellerName"]
                        + "</b><br />"
                        + "Merchant: <b>" + full["MerchantName"] + "</b><br />"
                        + "Branch: <b>" + full["BranchName"] + "</b>";
                    return renderMe;
                }
            },
            {
                data: "Status"
            },
            {
                data: "DateActivated", "render": function (data, type, full, meta) {
                    if (data != null) {
                        return showDateInWords(data);
                    } else {
                        return data;
                    }
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status;

                    if (data == 1) {
                        status = "Deactivate";
                    } else {
                        status = "Activate";
                    }

                    if (status == "Deactivate") {
                        var link = "<input type='button' style='width:100px;' id='mbpStatus" + posId + "' class='btn btn-success' merchantbranchposid='" + posId + "' value='" + status + "' onclick='updateMerchantBranchPOSStatus(this);'>";
                    } else if (status == "Activate") {
                        var link = "<input type='button' style='width:100px;' id='mbpStatus" + posId + "' class='btn btn-default' merchantbranchposid='" + posId + "' value='" + status + "' onclick='updateMerchantBranchPOSStatus(this);'>";
                    }

                    return link;
                }
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No POS(s) Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/GetPOSByBranch',
            type: "POST",
            data: { "bId": BranchId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshSwitchTable(SwitchId) {
    var id;
    $('#swithTbl').dataTable().fnDestroy();
    $('#swithTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "SwitchId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    id = data;
                    return data;
                }
            },
            { data: "SwitchName" },
            {
                data: "IsAddressRequired", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Inactive";
                    } else {
                        data = "Active";
                    }
                    var renderMe1 = "<label>" + data + "</label>";
                    return renderMe1;
                }
            },
            {
                data: "IsActive", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Activated";
                    } else {
                        data = "Deactivated";
                    }
                    var renderMe1 = "<label id='" + id + "td" + "'>" + data + "</label>";
                    return renderMe1;
                }
            },
            {
                data: "IsActive", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Deactivate";
                    } else {
                        data = "Activate";
                    }
                    var renderer = "<input id='" + id + "btn" + "' style='width:100px;' data-value='" + data + "' type='button' class='btn btn-default btn-rounded update' value=" + data + " onclick='updateStatus(this);'></input>";
                    return renderer;
                }
            }

        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Switch Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/SwitchList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshSwitchPartnerTable(SwitchId) {
    var id;
    pId = SwitchId;
    $('#switchTbl').dataTable().fnDestroy();
    $('#switchTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "EnabledToPartner", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Deactivate";
                    } else {
                        data = "Activate";
                    }
                    var renderMe1 = "<label>" + data + "</label>";
                    isEnable = data;
                    return renderMe1;
                }
            },
            {
                data: "SwitchId", "render": function (data, type, full, meta) {
                    id = data;
                    if (isEnable != "Activate") {
                        data = "Disable";
                    } else {
                        data = "Enable";
                    }
                    var renderMe = "<input id='" + id + "' class='btn btn-default btn-rounded enable' type='button' name='pickMeUp' value='" + data + "' onclick='updatePartnerSwitch(this);'/>";
                    return renderMe;
                }
            },
            {
                data: "SwitchId", "render": function (data, type, full, meta) {
                    sId = data;
                    return data;
                }
            },
            {
                data: "SwitchName"
            },
            {
                data: "AddressRequired", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Yes";
                    }
                    else {
                        data = "No";
                    }
                    return data;
                }
            },
            {
                data: "EnabledToPartner", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Yes";
                    } else {
                        data = "No";
                    }
                    var renderMe1 = "<label id='" + id + "enable" + "' data-value='" + data + "'>" + data + "</label>";
                    isEnable = data;
                    return renderMe1;
                }
            }

        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [2],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "Select a Partner to View Switch List"
        },
        "ajax": {
            url: rootDir + 'Ajax/SwitchPartnerLinkList',
            type: "POST",
            data: { 'id': SwitchId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshPOSAssignedMIDsTable(pId) {
    var mId;
    posId = pId;
    var midId;
    $('#POSTbl').dataTable().fnDestroy();
    $('#POSTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MidId", "render": function (data, type, full, meta) {
                    midId = data;
                    return data;
                }
            },
            {
                data: "MerchantBranchPOSsId", "render": function (data, type, full, meta) {
                    mId = data;

                    var renderMe = "<input midId ='" + midId + "' type='button' name='pickMeUp' data-value='" + data + "' value='Remove' class='btn btn-default btn-rounded' onclick='removeMids(this);'/>";
                    return renderMe;
                }
            },
            { data: "MidName" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    var renderer = "<label>" + status + "</label>";
                    return renderer;
                }
            },
            {
                data: "IsDeleted"
            },
            {
                data: "Currency"
            }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Rows Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/POSAssignedMID',
            type: "POST",
            data: { 'id': pId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMidsTable(mId) {
    $('#MIDsTbl').dataTable().fnDestroy();

    $('#MIDsTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "EncryptedCardTypeId", "render": function (data, type, full, meta) {

                    cid = data;
                    return data;
                }
            },
            {
                data: "EncryptedMIDId", "render": function (data, type, full, meta) {
                    var renderMe = "<input cid='" + cid + "' id='radio' name='radio' type='radio' data-value='" + data + "' onclick='readyForActionAssignMid(this);' />";
                    return renderMe;
                }
            },
            { data: "MIDName" },
            { data: "Switch" },
            { data: "CardType" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No MIDs Found. Please select a Merchant"
        },
        "ajax": {
            url: rootDir + 'Ajax/MIDList',
            data: { "id": mId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMasterDeviceTable() {
    var id;
    $('#deviceTbl').dataTable().fnDestroy();

    $('#deviceTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MasterDeviceId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    var id = data;
                    return renderMe;
                }
            },
            { data: "DeviceName" },
            { data: "Manufacturer" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Device yet, would you like to <a href='/Device'>register</a>?"
        },
        "ajax": {
            url: rootDir + 'Ajax/MasterDeviceList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshSNTablebyMasterDevice(masterDeviceId) {
    var id;
    $('#deviceTbl').dataTable().fnDestroy();

    $('#deviceTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "DeviceId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    var id = data;
                    return renderMe;
                }
            },
            { data: "SerialNumber" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Device yet, would you like to <a href='/Device'>register</a>?"
        },
        "ajax": {
            url: rootDir + 'Ajax/SerialNumbersListBeyMasterDevice',
            type: "POST",
            data: { 'id': masterDeviceId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshEmailServerTable(PartnerId) {
    var id;
    pId = PartnerId;
    var enable;
    $('#emailserverTbl').dataTable().fnDestroy();

    $('#emailserverTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "EmailServerId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    id = data;
                    return renderMe;
                }
            },
            { data: "Email" },
            { data: "Name" },
            { data: "Host" },
            { data: "Port" },
            { data: "Username" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    if (data == true) {
                        data = "Deactivate";
                    } else {
                        data = "Activate";
                    }
                    enable = data;
                    var render = "<button id='" + id + "' style='width:100px;' data-value='" + data + "' type='button' class='btn btn-default btn-rounded' value=" + data + " onclick='updateEmailServer(this);'>" + data + "</button>";
                    return render;
                }
            },
            {
                data: "IsDeleted", render: function (data, type, full, meta) {
                    data = "Delete";
                    var renderer = "<button enable='" + enable + "' id='" + id + "delete" + "' style='width:100px;' data-value='" + data + "' class='btn btn-default btn-rounded delete' onclick='updateEmailServer(this);'>" + data + "</button>";
                    return renderer;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Email Server found for this merchant"
        },
        "ajax": {
            url: rootDir + 'Ajax/EmailServerList',
            type: "POST",
            data: { 'pId': PartnerId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshUsersTable(parentId, parentTypeId) {

    $('#usersTbl').dataTable().fnDestroy();

    $('#usersTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "AccountId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    id = data;
                    return renderMe;
                }
            },
            { data: "User.FirstName" },
            { data: "User.ContactInformation.City" },
            { data: "User.ContactInformation.StateProvince" },
            { data: "User.ContactInformation.Country.CountryName" },
            { data: "User.ContactInformation.PrimaryContactNumber" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    if (data == true) {
                        data = "Deactivate";
                    } else {
                        data = "Activate";
                    }
                    var renderer = "<button id='" + id + "' style='width:100px;' data-value='" + data + "' type='button' class='btn btn-default btn-rounded' value=" + data + " onclick='updateUserStatus(this);'>" + data + "</button>";
                    return renderer;
                }
            },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    if (data == true) {
                        data = "User Active";
                    } else {
                        data = "User Account Deactivated";
                    }
                    var render = "<label id='" + id + "enable" + "'>" + data + "</label>"
                    return render;
                }
            },
            {
                data: "IsAccountAvailable", render: function (data, type, full, meta) {
                    if (data == true) {
                        data = "Unlock User";
                        var renderer = "<button id='" + id + "available" + "' style='width:100px;' data-value='" + data + "' type='button' class='btn btn-default btn-rounded' value=" + data + " onclick='updateUserAvailability(this);'>" + data + "</button>";
                        return renderer;
                    } else if (data == false) {
                        data = "Unlock User";
                        var renderer = "<button id='" + id + "' style='width:100px;' data-value='" + data + "' type='button' class='btn btn-default btn-rounded' value=" + data + " disabled='disabled'>" + data + "</button>";
                        return renderer;
                    }
                }
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Users Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/UsersList',
            type: "POST",
            data: { 'parentId': parentId, 'parentTypeId': parentTypeId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshReportsTable(pId, rId, mId, posId, bId, transTypeId, actionId, startDate, endDate) {
    var cardTypeId;
    var x;
    var currency;
    $('#transTbl').dataTable().fnDestroy();

    $('#transTbl').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "Currency", render: function (data, type, full, meta) {
                    currency = data;
                    return data;
                }
            },
            {
                data: "CardTypeId", render: function (data, type, full, meta) {
                    cardTypeId = data;
                    return data;
                }
            },
            { data: "CardType" },
            {
                data: "TransactionTypeId", render: function (data, type, full, meta) {
                    x = data;
                    if (data == 1) {
                        data = "Capture";
                    } else if (data == 2) {
                        data = "Pre Auth";
                    } else if (data == 3) {
                        data = "Purchased";
                    } else if (data == 4) {
                        data = "Void";
                    } else if (data == 5) {
                        data = "Refund";
                    } else if (data == 6) {
                        data = "Declined";
                    }

                    return data;
                }
            },
            {
                data: "TotalTransaction", render: function (data, type, full, meta) {
                    var link = "<input type='button' data-cardtype='" + cardTypeId + "' data-currency='" + currency + "' data-currency='" + currency + "' data-value='" + x + "' value='View Transactions Details' id='" + tId + "' class='btn btn-default btn-rounded' onclick='onClickTransaction(this);' />";
                    return link;
                }
            },
            { data: "Currency" },
            { data: "TotalCount" },
            { data: "TotalAmount" }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/TransactionAttemptList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshDebitReportsTable(pId, rId, mId, posId, bId, transTypeId, actionId, startDate, endDate) {

    var x;
    var currency;
    $('#transTbl2').dataTable().fnDestroy();

    $('#transTbl2').DataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "Currency", render: function (data, type, full, meta) {
                    currency = data;
                    return data;
                }
            },
            {
                data: "TransactionTypeId", render: function (data, type, full, meta) {
                    x = data;
                    if (data == 6) {
                        data = "Declined";
                    } else if (data == 3) {
                        data = "Purchased";
                    }
                    return data;
                }
            },
            {
                data: "TotalTransaction", render: function (data, type, full, meta) {
                    var link = "<input type='button' data-currency='" + currency + "' data-value='" + x + "' value='View Transactions Details' id='" + tId + "' class='btn btn-default btn-rounded' onclick='onClickDebitTransaction(this);' />";
                    return link;
                }
            },
            { data: "Currency" },
            { data: "TotalTransaction" },
            { data: "TotalAmount" }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/TransactionAttemptDebitList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshTransactionTable(pId, rId, mId, posId, bId, cardTypeId, transTypeId, actionId, currencyname, startDate, endDate) {

    $('#transactionTbl').dataTable().fnDestroy();

    $('#transactionTbl').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": false,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            { data: "TransNumber" },
            { data: "TotalAmount" },
            { data: "Reference" },
            {
                data: "TransactionTypeId", render: function (data, type, full, meta) {
                    x = data;
                    if (data == 1) {
                        data = "Capture";
                    } else if (data == 2) {
                        data = "Pre Auth";
                    } else if (data == 3) {
                        data = "Purchased";
                    } else if (data == 4) {
                        data = "Void";
                    } else if (data == 5) {
                        data = "Refund";
                    } else if (data == 6) {
                        data = "Declined";
                    }

                    return data;
                }
            },
            { data: "CardType" },
            { data: "NameOnCard" },
            { data: "CardNumber" },
            { data: "POS" },
            { data: "Location" },
            { data: "MerchantName" },
            {
                data: "DateReceived", "render": function (data, type, full, meta) {
                    if (data != null) {
                        return showDateInWords(data);
                    } else {
                        return data;
                    }
                }
            },
            { data: "TransactionTime" },
            {
                data: "ImageSource", "render": function (data, type, full, meta) {
                    if (data == null) {
                        data = "No Signature";
                        return data;
                    } else {
                        var img = "<img style='height:50px; width:100px' src='data:image/jpg;base64," + data + "'>";
                        return img;
                    }
                }
            },
            { data: "POSEntryMode" },
            { data: "TraceNumber" },
            {
                data: "LatLng", "render": function (data, type, full, meta) {

                    var LatLng = data.split(",");
                    var lat = LatLng[0];
                    var long = LatLng[1].trim();
                    var location = "<input type='button' value='Show Location' class='btn btn-default btn-rounded' onclick='callMap(" + lat + "," + long + ");' />";
                    return location;
                }
            },
            { data: "Notes" },
        ],
        "columnDefs": [
            {
                "targets": [2],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [3],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [4],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/TransactionList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'cardTypeId': cardTypeId, 'transTypeId': transTypeId, 'actionId': actionId, 'currenyName': currencyname, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshDebitTransactionTable(pId, rId, mId, posId, bId, transTypeId, actionId, currencyname, startDate, endDate) {

    $('#transactionTbl2').dataTable().fnDestroy();

    $('#transactionTbl2').dataTable({
        "autoWidth": true,
        "scrollX": false,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            { data: "TransNumber" },
            { data: "TotalAmount" },
            { data: "Reference" },
            {
                data: "TransactionTypeId", render: function (data, type, full, meta) {
                    x = data;
                    if (data == 1) {
                        data = "Capture";
                    } else if (data == 2) {
                        data = "Pre Auth";
                    } else if (data == 3) {
                        data = "Purchased";
                    } else if (data == 4) {
                        data = "Void";
                    } else if (data == 5) {
                        data = "Refund";
                    } else if (data == 6) {
                        data = "Declined";
                    }

                    return data;
                }
            },
            { data: "NameOnCard" },
            { data: "CardNumber" },
            { data: "POS" },
            { data: "Location" },
            { data: "MerchantName" },
            {
                data: "DateReceived", "render": function (data, type, full, meta) {
                    if (data != null) {
                        return showDateInWords(data);
                    } else {
                        return data;
                    }
                }
            },
            {
                data: "LatLng", "render": function (data, type, full, meta) {
                    var LatLng = data.split(",");
                    var lat = LatLng[0];
                    var long = LatLng[1].trim();
                    var location = "<input type='button' value='Show Location' class='btn btn-default btn-rounded' onclick='callMap(" + lat + "," + long + ");' />";
                    return location;
                }
            }
        ],
        "columnDefs": [
            {
                "targets": [3],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [4],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [5],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/TransactionDebitList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'currenyName': currencyname, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshTopSellingMerchant() {
    //var src = rootDir + "img/filetree/spinner.gif";
    //$.blockUI({ message: '<h4><img src="' + src + '"> Loading ...</h4>' });
    $('#topMerchantTbl').dataTable().fnDestroy();

    $('#topMerchantTbl').DataTable({
        "autoWidth": true,
        "scrollX": false,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            { data: "MerchantName" },
            { data: "PartnerCompany" },
            {
                data: "TotalAmount", "render": function (data, type, full, meta) {
                    var tAmount = addCommas(data);
                    return tAmount;
                }
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants Found"
        },
        "ajax": {
            url: rootDir + 'Ajax/TopMerchantList',
            type: "POST",
            error: function (data) {
                $(document).ajaxStop($.unblockUI);

                $('#topMerchantTbl').dataTable().fnDestroy();
                $('#topMerchantTbl').dataTable({
                    "bSort": false,
                    "bPaginate": false,
                    "bFilter": false,
                    "bInfo": false,
                    //"sDom": '<"clear">',
                    "columns": [
                        { data: "MerchantName" },
                        { data: "PartnerCompany" },
                        { data: "TotalAmount" }
                    ],
                    "oLanguage": {
                        "sZeroRecords": "An Error occured, please refresh the page."
                    }
                });
            }
        }, "fnDrawCallback": function (oSettings) {
            $(document).ajaxStop($.unblockUI);
        }
    });
}

function refreshRequestedMerchantsTable(pId) {
    var id;
    $('#rMerchantsTbl').dataTable().fnDestroy();

    $('#rMerchantsTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "RequestedMerchantId", "render": function (data, type, full, meta) {
                    id = data;
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "MerchantName" },
            {
                data: "DateCreated", "render": function (data, type, full, meta) {
                    return showDateInWords(data);
                }
            },
            {
                data: "Status", "render": function (data, type, full, meta) {
                    if (data == true) {
                        data = "Approved";
                    } else {
                        data = "Pending";
                    }

                    return data;
                }
            },
            {
                data: "IsActive", "render": function (data, type, full, meta) {
                    if (data == true) {
                        var input = "<input type='button' class='btn btn-default' value='Activate' disabled='disabled'>";
                    } else {
                        var input = "<input type='button' class='btn btn-default' data-value='" + id + "' value='Activate' onclick='registerRequestedMerchant(this);'>";
                    }

                    return input;
                }
            }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + "Ajax/RequestedMerchantList",
            type: "POST",
            error: function (data) {
                window.location.href = "?err=1";
            }
        }
    });
}

$("#close").click(function () {
    $("#message-box-danger").hide();
});

$("#export").click(function () {
    exportToCSV(partner, reseller, merchant, pos, branch, cType, etType, eAction, curr, eStart, eEnd);
});

function exportToCSV(pId, rId, mId, posId, bId, cardTypeId, transTypeId, actionId, currencyname, startDate, endDate) {

    var transactionType = document.getElementById('ddltranstype').value;
    var typeUrl;

    if (transactionType == 5) {
        typeUrl = rootDir + "Ajax/TransactionList2";
    } else if (transactionType == 4) {
        cardTypeId = 0;
        typeUrl = rootDir + "Ajax/TransactionDebitList2";
    } else if (transactionType == 0) {
        transTypeId = etType;
        typeUrl = rootDir + "Ajax/TransactionList2";
    }

    $.ajax({
        url: typeUrl,
        type: "POST",
        data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'cardTypeId': cardTypeId, 'transTypeId': transTypeId, 'actionId': actionId, 'currenyName': currencyname, 'startDate': startDate, 'endDate': endDate },
        success: function (data) {

            var csv = JSON2CSV(data);

            var downloadLink = document.createElement("a");
            var blob = new Blob(["\ufeff", csv]);
            var url = URL.createObjectURL(blob);
            downloadLink.href = url;
            downloadLink.download = "ReportsData.xls";

            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);
        },
        error: function (data) {
            window.location.href = "Home?err=1";
        }
    });
}

function showDateInWords(dt) {

    var dys = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    var mns = new Array('Jaunary', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December');

    var sam = dt.split('/');
    var day, month, year
    day = sam[1]; month = sam[0]; year = sam[2];
    var nd = new Date()
    nd.setYear(year);
    nd.setMonth(month - 1);
    nd.setDate(day);

    var dt = nd.getDay()
    var fin = dys[dt]

    var daynumber = nd.getDate()

    if (daynumber == 10 && daynumber == 19)
        daynumber = daynumber + 'th'
    else if ((daynumber % 10) == 1)
        daynumber = daynumber + 'st'
    else if ((daynumber % 10) == 2)
        daynumber = daynumber + 'nd'
    else if ((daynumber % 10) == 3)
        if (daynumber == 13)
            daynumber = daynumber + 'th'
        else
            daynumber = daynumber + 'rd'
    else
        daynumber = daynumber + 'th'

    fin = fin + ', ' + daynumber + ' ' + mns[nd.getMonth()] + ' ' + sam[2]

    return fin;
}

function JSON2CSV(objArray) {
    var transactionType = document.getElementById('ddltranstype').value;
    var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    var title = '';
    if (transactionType == "5") {
        title = '"Customer Name",Total Amount,Transaction Number,Reference Number(Sales),POS Name,Branch Name,Merchant Name,Date Received,POS Entry Mode';
    } else if (transactionType == "4") {
        title = '"Transaction Number",Total Amount,Reference Number(Sales),Branch Name,Merchant Name,POS Name,Date Received';
    } else if (transactionType == "0") {
        title = '"Customer Name",Total Amount,Transaction Number,Reference Number(Sales),POS Name,Branch Name,Merchant Name,Date Received,POS Entry Mode';
    }

    var line = '';
    var result = '';
    var columns = [];

    var head = array[0];
    if (true) {
        var i = 0;
        for (var key in array[0]) {
            var keyString = key + "";
            keyString = '"' + keyString.replace(/"/g, '""') + '",';
            columns[i] = key;
            line += keyString;
            i++;
        }
    } else {
        var i = 0;
        for (var key in array[0]) {
            keyString = key + ',';
            columns[i] = key;
            line += keyString;
            i++;
        }
    }

    if (true) {
        line = line.slice(0, 0);
        result += title + line + '\r\n';
    }


    for (var i = 0; i < array.length; i++) {
        var line = '';

        if (true) {

            for (var j = 0; j < columns.length; j++) {
                var value = array[i][columns[j]] ? array[i][columns[j]] : '';
                var valueString = value + "";
                line += '"' + valueString.replace(/"/g, '""') + '",';
            }
        } else {
            for (var j = 0; j < columns.length; j++) {
                var value = array[i][columns[j]] ? array[i][columns[j]] : '';
                var valueString = value + ',';
                line += valueString;
            }
        }

        line = line.slice(0, -1);
        result += line + '\r\n';
    }

    return result;

}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function block() {
    $.ajax({ url: rootDir + 'MerchantBranchPOSs', cache: false });
}

$(document).ready(function () {
    $("#dvActionKeys").hide();

    $("#ddlResellers").on("change", function () {
        $.ajax({
            url: rootDir + 'Ajax/GetMerchants',
            type: "POST",
            data: { 'ResellerId': this.value },
            success: function (data) {
                data = data[0].data;
                $('#ddlMerchants').empty();
                for (var i = 0; i < data.length; i++) {
                    $('#ddlMerchants').append($("<option></option>").attr("value", data[i].Value).text(data[i].Text));
                }

                $('#ddlMerchants').change();
            }
        });
    });

});