var chosenI = 0;
var posId = 0;
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

var userType;

var errorMsg;

function getMerchantIds(obj) {
    var id = obj;

    $.ajax({
        url: rootDir + "GetMerchantBoarding",
        type: 'POST',
        data: { "id": id },
        success: function (data) {
            alert('success');// alert the data from the server
        },
        error: function () {
        }
    });
}

function readyForAction(obj) {
    chosenI = obj.getAttribute("data-value");
    $("#dvActionKeys").show();
}

function redirectAction(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "/" + chosenI;
}

function readyForActionAssignMid(obj) {
    chosenI = obj.getAttribute("data-value");
    cid = obj.getAttribute('cid');
    $("#dvActionKeys").show();
}

function redirectActionMidAssign(obj) {
    var mId = document.getElementById('ddlMerchants').value;

    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI + "&" + "mId=" + mId + "&" + "cId=" + cid;
}

function redirectActionMidRemove(obj) {
    var mId = document.getElementById('ddlMerchants').value;
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "?id=" + chosenI + "&" + "mId=" + mId;
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
    
    userType = document.getElementById('userType').value;
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var pId = document.getElementById('hdnpId').value;
    var tType = obj.getAttribute("data-value");
    var currency = obj.getAttribute("data-currency");
    var cardTypeId = obj.getAttribute("data-cardtype");
    var p = document.getElementById('ddlpartners');
    var r = document.getElementById('ddlresellers');
    var m = document.getElementById('ddlmerchants');
    
    var b = document.getElementById('ddlbranches');
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;

    etType = tType;
    cType = cardTypeId;
    curr = currency;
    branch = b;
    pos = mp;
    eAction = action;

    if (p != null) {
        p = document.getElementById('ddlpartners').value;
    } else { p = 0; }

    if (r != null) {
        r = document.getElementById('ddlresellers').value;
    } else { r = 0; }

    if (m != null) {
        m = document.getElementById('ddlmerchants').value;
    } else { m = 0; }

    if (b != null) {
        b = document.getElementById('ddlbranches').value;
    } else { b = 0; }

    partner = p > 0 ? p : pId;
    reseller = r;
    merchant = m;
    branch = b;

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

    if (p > 0) {
        refreshTransactionTable(p, r, m, mp, b, cardTypeId, tType, action, currency, start, end);
    } else {
        if (userType == "reseller") {
            refreshTransactionTable(0, pId, m, mp, b, cardTypeId, tType, action, currency, start, end);
        } else if (userType == "merchant") {
            refreshTransactionTable(0, 0, pId, mp, b, cardTypeId, tType, action, currency, start, end);
        } else {
            refreshTransactionTable(pId, r, m, mp, b, cardTypeId, tType, action, currency, start, end);
        }
    }

    $(".1st").hide();
    $(".2nd").show();
}

function onClickDebitTransaction(obj) {
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var tType = obj.getAttribute("data-value");
    var hType = document.getElementById('hdnType').value;
    var currency = obj.getAttribute("data-currency");
    var pId = document.getElementById('hdnpId').value;
    var r = document.getElementById('ddlresellers').value;
    var m = document.getElementById('ddlmerchants').value;
    var b = document.getElementById('ddlbranches').value;
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;

    etType = tType;
    curr = currency;

    branch = b;
    pos = mp;
    eAction = action;

    if (r != null) {
        r = document.getElementById('ddlresellers').value;
    } else { r = 0; }

    if (m != null) {
        m = document.getElementById('ddlmerchants').value;
    } else { m = 0; }

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

    if (pId != 0) {
        $("#tTbl1").hide();
        $("#tTbl2").show();
        refreshDebitTransactionTable(pId, r, m, mp, b, tType, action, currency, start, end);
        $(".1st").hide();
        $(".2nd").show();
    } else {
        if (hType == "reseller") {
            refreshDebitTransactionTable(0, pId, m, mp, b, tType, action, currency, start, end);
        } else if (hType == "merchant") {
            refreshDebitTransactionTable(0, 0, pId, mp, b, tType, action, currency, start, end);
        } else {
            refreshDebitTransactionTable(0, r, m, mp, b, tType, action, currency, start, end);
        }
    }

    $("html, body").animate({ scrollTop: 0 }, 250);
}

function sendActivationCode(posId, actCode) {
    var src = rootDir + "img/filetree/spinner.gif";
    $.blockUI({ message: '<h4><img src="' + src + '"> Sending Activation Code ...</h4>' });
    block();
    $.ajax({
        url: rootDir + "MerchantBranchPOSs/SendActivationCode",
        type: 'POST',
        data: { "posId": posId, "actCode": actCode },
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

function refreshPartnersTable(partnerId) {
    $('#partnersTbl').dataTable().fnDestroy();

    $('#partnersTbl').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "iDisplayLength": 10,
        "columns": [
            {
                data: "PartnerId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
            },
            { data: "CompanyName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            { data: "ParentPartner.CompanyName" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Partners yet, would you like to <a href='Partners/Registration'>register</a> a new partner?"
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

function refreshResellerTable(resellerId) {
    $('#resellersTbl').dataTable().fnDestroy();

    $('#resellersTbl').dataTable({
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
            "sZeroRecords": "No Resellers yet, would you like to <a href='Resellers/Registration'>register</a> a new reseller?"
        },
        "ajax": {
            url: rootDir + 'Ajax/ResellerList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantsTable(MerchantId) {
    var mId;
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
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
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'Ajax/MerchantList',
            data: { "id": MerchantId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}


function refreshMerchantsforFeaturesTable() {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
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
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'Ajax/MerchantforFeatureList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshBranchTable(mId) {
    var bId;
    $('#BranchesTbl').dataTable().fnDestroy();
    $('#BranchesTbl').dataTable({
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
    $('#POSTbl').dataTable().fnDestroy();
    $('#POSTbl').dataTable({
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

function refreshPOSAssignedMIDsTable(pId) {
    var mId;
    posId = pId;
    var midId;
    $('#POSTbl').dataTable().fnDestroy();
    $('#POSTbl').dataTable({
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

function refreshMidsTable(MIDId) {
    $('#MIDsTbl').dataTable().fnDestroy();

    $('#MIDsTbl').dataTable({
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
                data: "CardTypeId", "render": function (data, type, full, meta) {
                    cid = data;
                    return data;
                }
            },
            {
                data: "MIDId", "render": function (data, type, full, meta) {
                    var renderMe = "<input cid='" + cid + "' name='radio' type='radio' data-value='" + data + "' onclick='readyForActionAssignMid(this);' />";
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
            data: { "id": MIDId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantDeviceTable(merchantId) {
    $('#partnersTbl').dataTable().fnDestroy();

    $('#partnersTbl').dataTable({
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
            { data: "CompanyName" },
            { data: "PrimaryContactNumber" },
            { data: "City" },
            { data: "ParentPartner.CompanyName" },
            {
                data: "IsActive", render: function (data, type, full, meta) {
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Partners yet, would you like to <a href='Partners/Registration'>register</a> a new partner?"
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

function refreshUsersTable(parentId, parentTypeId) {

    $('#usersTbl').dataTable().fnDestroy();

    $('#usersTbl').dataTable({
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
    var y;
    var currency;
    $('#transTbl').dataTable().fnDestroy();

    $('#transTbl').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": false,
        "sPaginationType": "full_numbers",
        "bPaginate": false,
        "bInfo": false,
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
                data: "TransType", render: function (data, type, full, meta) {
                    y = data;
                    if (data == 3) {
                        data = "Manual Credit";
                    } else if (data == 5) {
                        data = "Credit";
                    } else if (data == 4) {
                        data = "Debit";
                    } else if (data == 9) {
                        data = "EMV";
                    } 

                    return data;
                }
            },
            {
                data: "TotalTransaction", render: function (data, type, full, meta) {
                    var link = "<input type='button' data-cardtype='" + cardTypeId + "' data-currency='" + currency + "' data-value='" + x + "' data-transtype='" + y + "' value='View Transactions Details' id='" + tId + "' class='btn btn-default btn-rounded' onclick='onClickTransaction(this);' />";
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
            { data: "AuthNumber" },
            { data: "InvoiceNumber" },
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

function refreshDebitReportsTable(pId, rId, mId, posId, bId, transTypeId, actionId, startDate, endDate) {

    var x;
    var currency;
    $('#transTbl2').dataTable().fnDestroy();

    $('#transTbl2').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": false,
        "sPaginationType": "full_numbers",
        "bPaginate": false,
        "bInfo": false,
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

function refreshDebitTransactionTable(pId, rId, mId, posId, bId, transTypeId, actionId, currencyname, startDate, endDate) {

    $('#transactionTbl2').dataTable().fnDestroy();

    $('#transactionTbl2').dataTable({
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

function refreshTopSellingMerchant(pId) {
    var src = rootDir + "img/filetree/spinner.gif";
    $.blockUI({ message: '<h4><img src="' + src + '"> Loading ...</h4>' });

    $('#topMerchantTbl').dataTable().fnDestroy();

    $('#topMerchantTbl').dataTable({
        "bSort": false,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "sDom": '<"clear">',
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
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + "Ajax/TopMerchantList",
            type: "POST",
            data: { "pId": pId },
            error: function (data) {
                $('#topMerchantTbl').dataTable().fnDestroy();
                $('#topMerchantTbl').dataTable({
                    "bSort": false,
                    "sDom": '<"clear">',
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

function refreshTopSellingMerchantForReseller(rId) {
    $('#topMerchantTbl').dataTable().fnDestroy();

    $('#topMerchantTbl').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "sDom": '<"clear">',
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
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + "Ajax/TopMerchantListByReseller",
            type: "POST",
            data: { "rId": rId },
            error: function (data) {
                window.location.href = "?err=1";
            }
        }
    });
}

function refreshTopSellingMerchantForMerchant(mId) {
    $('#topBranchTbl').dataTable().fnDestroy();

    $('#topBranchTbl').dataTable({
        "autoWidth": true,
        "scrollX": true,
        "fixedColumns": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "sDom": '<"clear">',
        "columns": [
            { data: "MerchantBranchName" },
            { data: "PartnerCompany" },
            {
                data: "TotalAmount", "render": function (data, type, full, meta) {
                    var tAmount = addCommas(data);
                    return tAmount;
                }
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: rootDir + "Ajax/TopMerchantListByMerchant",
            type: "POST",
            data: { "mId": mId },
            error: function (data) {
                window.location.href = "?err=1";
            }
        }, "fnDrawCallback": function (oSettings) {
            $(document).ajaxStop($.unblockUI);
        }
    });
}

function refreshTableRequestedMerchants() {
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
            "sZeroRecords": "No Request Found"
        },
        "ajax": {
            url: rootDir + "Ajax/RequestedMerchantsList",
            type: "POST",
            data: { "parentId": pId },
            error: function (data) {
                window.location.href = "?err=1";
            }
        }
    });
}


$("#close").click(function () {
    $("#message-box-danger").hide();
});

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

$("#export").click(function () {
    exportToCSV(partner, reseller, merchant, pos, branch, cType, etType, eAction, curr, eStart, eEnd);
});

function exportToCSV(pId, rId, mId, posId, bId, cardTypeId, transTypeId, actionId, currencyname, startDate, endDate) {
    
    var transactionType = document.getElementById('ddltranstype').value;
    var typeUrl;

    if (transactionType == 5 || transactionType == 3) {
        typeUrl = rootDir + 'Ajax/TransactionList2';
    } else if (transactionType == 4) {
        cardTypeId = 0;
        typeUrl = rootDir + 'Ajax/TransactionDebitList2';
    } else if (transactionType == 0) {
        typeUrl = rootDir + "Ajax/TransactionList2";
        transTypeId = etType;
    }
    
    if (userType == "reseller") {
        rId = pId;
    } else if (userType == "merchant") {
        mId = pId;
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
    title = '';
    if (transactionType == "5") {
        title = '"Customer Name",Total Amount,Transaction Number,Reference Number(Sales),POS Name,Branch Name,Merchant Name,Date Received,POS Entry Mode, Auth Number';
    } else if (transactionType == "4") {
        title = '"Transaction Number",Total Amount,Reference Number(Sales),Branch Name,Merchant Name,POS Name,Date Received';
    } else if (transactionType == "0") {
        title = '"Customer Name",Total Amount,Transaction Number,Reference Number(Sales),POS Name,Branch Name,Merchant Name,Date Received,POS Entry Mode, Auth Number';
    } else if (transactionType == "3") {
        title = '"Customer Name",Total Amount,Transaction Number,Invoice Number(Sales),POS Name,Branch Name,Merchant Name,Date Received,POS Entry Mode, Auth Number, Trans Number';
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