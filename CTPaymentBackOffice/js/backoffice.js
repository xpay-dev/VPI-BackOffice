var chosenI = 0;
var posId = 0;
var tId;

function getMerchantIds(obj)
{
    var id = obj;
    alert(id);

    $.ajax({
        url: "GetMerchantBoarding",
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

function redirectActionMidAssign(obj) {
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
            //alert('Geocoder failed due to: ' + status);
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
        url: "Users/UpdateUserStatus",
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

function onClickTransaction(obj) {
    
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var pId = document.getElementById('hdnpId').value;
    var tType = obj.getAttribute("data-value");
    var currency = obj.getAttribute("data-currency");
    var p = document.getElementById('ddlpartners');
    var r = document.getElementById('ddlresellers');
    var m = document.getElementById('ddlmerchants');
    
    if (p != null) {
        p = document.getElementById('ddlpartners').value;
    } else { p = 0; }

    if (r != null) {
        r = document.getElementById('ddlresellers').value;
    } else { r = 0; }

    if (m != null) {
        m = document.getElementById('ddlmerchants').value;
    } else { m = 0; }

    var b = document.getElementById('ddlbranches').value;
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;
    
    if (start == "" && end == "") {
        start = "1/1/0001";
        end = "1/1/0001";
    } else if (start == "") {
        start = "1/1/0001";
    } else if (end == "") {
        end = "1/1/0001";
    }

    $("#tTbl1").show();
    $("#tTbl2").hide();

    if (p != 0) {
        
        refreshTransactionTable(p, r, m, mp, b, tType, action, currency, start, end);
    } else {
        
        refreshTransactionTable(pId, r, m, mp, b, tType, action, currency, start, end);
    }
    
    $(".1st").hide();
    $(".2nd").show();
}

function onClickDebitTransaction(obj) {
    var start = document.getElementById('datepickerstart').value;
    var end = document.getElementById('datepickerend').value;
    var tType = obj.getAttribute("data-value");
    var currency = obj.getAttribute("data-currency");
    var pId = document.getElementById('hdnpId').value;
    var r = document.getElementById('ddlresellers').value;
    var m = document.getElementById('ddlmerchants').value;
    var b = document.getElementById('ddlbranches').value;
    var mp = document.getElementById('ddlpos').value;
    var action = document.getElementById('ddlactions').value;


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
    if (pId != 0) {
        $("#tTbl1").hide();
        $("#tTbl2").show();
        refreshDebitTransactionTable(pId, r, m, mp, b, tType, action, currency, start, end);
        $(".1st").hide();
        $(".2nd").show();
    } else {
        refreshDebitTransactionTable(0, r, m, mp, b, tType, action, currency, start, end);
    }


    $("html, body").animate({ scrollTop: 0 }, 250);
}

function sendActivationCode(posId, actCode) {
    $.blockUI({ message: '<h4><span class="fa fa-cog fa-spin"></span> Sending Activation Code ...</h4>' });
    block();
    $.ajax({
        url: "MerchantBranchPOSs/SendActivationCode",
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
            url: '/Ajax/PartnerList',
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
            url: '/Ajax/ResellerList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantsTable(MerchantId) {
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
            url: '/Ajax/MerchantList',
            data: { "id": MerchantId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshMerchantBulkBoarding(ResellerId) {
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
                    var renderMe = "<input type='checkbox' id='chkMerchantId' name='pickMeUp' value='" + data + "' onclick='readyForAction(this);'/>";
                    return renderMe;
                }
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
            url: '/Ajax/MerchantMidsTobeAdded',
            data: { "id": ResellerId },
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
            url: '/Ajax/MerchantforFeatureList',
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshBranchTable(mId) {
    $('#BranchesTbl').dataTable().fnDestroy();
    $('#BranchesTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MerchantBranchId", "render": function (data, type, full, meta) {
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
                    var status = data == 1 ? "Active" : "Inactive";
                    return status;
                }
            }

        ],
        "oLanguage": {
            "sZeroRecords": "No Merchant Branch Found"
        },
        "ajax": {
            url: '/Ajax/GetBranchesByMerchant',
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
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No POS(s) Found"
        },
        "ajax": {
            url: '/Ajax/GetPOSByBranch',
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
            url: '/Ajax/POSAssignedMID',
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
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MIDId", "render": function (data, type, full, meta) {
                    var renderMe = "<input id='radio' name='radio' type='radio' data-value='" + data + "' onclick='readyForAction(this);' />";
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
        "oLanguage": {
            "sZeroRecords": "No MIDs Found. Please select a Merchant"
        },
        "ajax": {
            url: '/Ajax/MIDList',
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
            url: '/Ajax/PartnerList',
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
            url: '/Ajax/UsersList',
            type: "POST",
            data: { 'parentId': parentId, 'parentTypeId': parentTypeId },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshReportsTable(pId, rId, mId, posId, bId, transTypeId, actionId, startDate, endDate) {
    
    var x;
    var currency;
    $('#transTbl').dataTable().fnDestroy();

    $('#transTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": false,
        "bInfo" : false,
        "columns": [
            {
                data: "Currency", render: function (data, type, full, meta) {
                    currency = data;
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
                    var link = "<input type='button' data-currency='" + currency + "' data-value='" + x + "' value='View Transactions Detail' id='" + tId + "' class='btn btn-default btn-rounded' onclick='onClickTransaction(this);' />";
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
            url: '/Ajax/TransactionAttemptList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshTransactionTable(pId, rId, mId, posId, bId, transTypeId, actionId, currencyname, startDate, endDate) {

    $('#transactionTbl').dataTable().fnDestroy();

    $('#transactionTbl').dataTable({
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
            {
                data: "ImageSource", "render": function (data, type, full, meta) {
                    if (data == null) {
                        data = "No Signature";
                        return data;
                    } else {
                        var img = "<img style='height:50px; width:150px' src='data:image/jpg;base64," + data + "'>";
                        //var img = "<img src='data:image/jpg;base64, iVBORw0KGgoAAAANSUhEUgAAAGQAAABECAYAAAB3TpBiAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2hpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDowNjgwMTE3NDA3MjA2ODExOTJCMEM2OUIzNjAzMERBOCIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDowODI0NDNGRTU3NTQxMUUyQjQzMEM5RTU3RTJFODQxNyIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDowODI0NDNGRDU3NTQxMUUyQjQzMEM5RTU3RTJFODQxNyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M1LjEgTWFjaW50b3NoIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjE3NTYxNTAzRDIwNjgxMTkyQjBFQzY5MDhDMUQ4QjkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MDY4MDExNzQwNzIwNjgxMTkyQjBDNjlCMzYwMzBEQTgiLz4gPC9yZGY6RGVzY3Jp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'>";
                        //var img = "<img src='TestFolder/img.jpg'>";
                        return img;
                    }
                }
            },
            { data: "POSEntryMode" },
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
                "targets": [6],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: '/Ajax/TransactionList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'currenyName': currencyname, 'startDate': startDate, 'endDate': endDate },
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
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
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
                data: "ReturnCode", render: function (data, type, full, meta) {
                    x = data;
                    if (data == null) {
                        data = "Declined";
                    } else if (data == "00") {
                        data = "Purchased";
                    } else {
                        data = "Declined";
                    }
                    
                    if (x == null)
                    {

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
            url: '/Ajax/TransactionAttemptDebitList',
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
            //{
            //    data: "ImageSource", "render": function (data, type, full, meta) {
            //        if (data == null) {
            //            data = "No Signature";
            //            return data;
            //        } else {
            //            var img = "<img style='height:50px; width:150px' src='data:image/jpg;base64," + data + "'>";
            //            //var img = "<img src='data:image/jpg;base64, iVBORw0KGgoAAAANSUhEUgAAAGQAAABECAYAAAB3TpBiAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2hpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDowNjgwMTE3NDA3MjA2ODExOTJCMEM2OUIzNjAzMERBOCIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDowODI0NDNGRTU3NTQxMUUyQjQzMEM5RTU3RTJFODQxNyIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDowODI0NDNGRDU3NTQxMUUyQjQzMEM5RTU3RTJFODQxNyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M1LjEgTWFjaW50b3NoIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjE3NTYxNTAzRDIwNjgxMTkyQjBFQzY5MDhDMUQ4QjkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MDY4MDExNzQwNzIwNjgxMTkyQjBDNjlCMzYwMzBEQTgiLz4gPC9yZGY6RGVzY3Jp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'>";
            //            //var img = "<img src='TestFolder/img.jpg'>";
            //            return img;
            //        }
            //    }
            //},
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
                "targets": [6],
                "visible": false,
                "searchable": false
            }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: '/Ajax/TransactionDebitList',
            type: "POST",
            data: { 'pId': pId, 'rId': rId, 'mId': mId, 'posId': posId, 'bId': bId, 'transTypeId': transTypeId, 'actionId': actionId, 'currenyName': currencyname, 'startDate': startDate, 'endDate': endDate },
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshTopSellingMerchant(pId) {
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
            { data: "TotalAmount" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: '/Ajax/TopMerchantList',
            type: "POST",
            data: { "pId": pId },
            error: function (data) {
                window.location.href = "?err=1";
            }
        }
    });
}

function refreshTopSellingMerchantForReseller(rId) {
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
            { data: "TotalAmount" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: '/Ajax/TopMerchantListByReseller',
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
        "bSort": false,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "sDom": '<"clear">',
        "columns": [
            { data: "MerchantBranchName" },
            { data: "PartnerCompany" },
            { data: "TotalAmount" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Transaction(s) Found"
        },
        "ajax": {
            url: '/Ajax/TopMerchantListByMerchant',
            type: "POST",
            data: { "mId": mId },
            error: function (data) {
                window.location.href = "?err=1";
            }
        }
    });
}


$("#close").click(function () {
    $("#message-box-danger").hide();
});

function block() {
    $.ajax({ url: 'MerchantBranchPOSs', cache: false });
}

function showDateInWords(dt) {

    var dys = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday')
    var mns = new Array('Jaunary', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December')

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

$(document).ready(function () {
    $("#dvActionKeys").hide();

    $("#ddlResellers").on("change", function () {
        $.ajax({
            url: '/Ajax/GetMerchants',
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