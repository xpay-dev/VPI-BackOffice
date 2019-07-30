var chosenI = 0;

function readyForActionGetId(obj) {
    chosenI = obj.getAttribute("data-value");
    $("#divActionKeys").show();
}

function redirectActionToGetResponse(obj) {
    if (chosenI > 0)
        window.location = obj.getAttribute("data-link") + "/" + chosenI;
}

function refreshGetAllFilenameResponse() {
    $("#divActionKeys").hide();
    $('#FileResponseTbl').dataTable().fnDestroy();

    $('#FileResponseTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "FileId", "render": function (data, type, full, meta) {
                    var renderMe = "<input type='radio' name='pickMeUp' data-value='" + data + "' onclick='readyForActionGetId(this);'/>";
                    return renderMe;
                }
            },
            { data: "FileName" },
            { data: "DateReceived" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Files Available"
        },
        "ajax": {
            url: rootDir + 'AjaxMerchantOnBoard/MerchantOnBoardResponseFilenameList',
            type: "POST",
            error: function (data) {
                window.location.href = "GetFileNames?err=1";
            }
        }
    });
}

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
            { data: "CreditCard" },
            { data: "DebitCard" },
            {
                data: "NeedUpdateMerchant", render: function (data, type, full, meta) {
                    var status;
                    if (data == 0) {
                        status = full["MerchantInfo"];
                    } else {
                        status = "Need to update first the information of this Merchant.";
                    }
                    return status;
                }
            },
            { data: "Command" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'AjaxMerchantOnBoard/GetMerchantMidsTobeAdded',
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
            url: rootDir + 'AjaxMerchantOnBoard/ViewAddBulkData',
            data: { "ids": MerchantIds },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshUpdateMerchantBulkBoarding(ResellerId, ActionId) {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MidId", header: { content: "masterCheckbox", css: "center" }, "render": function (data, type, full, meta) {
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
                data: "ActionId", render: function (data, type, full, meta) {
                    var status;
                    if (data == 1) {
                        status =  "Merchant ID: <b>" + full["MerchantMids"]
                      + "</b><br />"
                      + "Merchant Address: <b>" + full["MerchantAddress"] + "</b>";
                    } else if (data == 2) {
                        status = "Card Type: <b>" + full["MidCardType"]
                      + "</b><br />"
                      + "Merchant ID: <b>" + full["MerchantMids"]
                      + "</b><br />"
                      + "Terminal ID: <b>" + full["TerminalId"] + "</b>";
                      + "</b><br />"
                      + "Set Like Terminal ID: <b>" + full["SetLikeTerminalId"] + "</b>";
                    } else {
                        status = "No data has been updated.";
                    }

                    return status;
                }
            },
            { data: "Command" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'AjaxMerchantOnBoard/GetMerchantMidsTobeUpdated',
            data: { "id": ResellerId, "actionId": ActionId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}

function refreshDeleteMerchantBulkBoarding(ResellerId, ActionId) {
    $('#MerchantsTbl').dataTable().fnDestroy();

    $('#MerchantsTbl').dataTable({
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "sPaginationType": "full_numbers",
        "bPaginate": true,
        "columns": [
            {
                data: "MidId", header: { content: "masterCheckbox", css: "center" }, "render": function (data, type, full, meta) {
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
                data: "ActionId", render: function (data, type, full, meta) {
                    var status;
                    if (data == 1) {
                        status = "Merchant ID: <b>" + full["MerchantMids"]
                      + "</b><br />"
                      + "Card Type: <b>" + full["MidCardType"] + "</b>";
                    } else if (data == 2) {
                        status = "Merchant ID: <b>" + full["MerchantMids"]
                      + "</b><br />"
                      + "Terminal ID: <b>" + full["TerminalId"] + "</b>";
                    } else if (data == 3) {
                        status = "Merchant ID: <b>" + full["MerchantMids"]
                      + "</b><br />"
                      + "Merchant Address: <b>" + full["MerchantAddress"] + "</b>";
                    } else {
                        status = "No data has been updated.";
                    }

                    return status;
                }
            },
            { data: "Command" }
        ],
        "oLanguage": {
            "sZeroRecords": "No Merchants available"
        },
        "ajax": {
            url: rootDir + 'AjaxMerchantOnBoard/GetMerchantMidsTobeDeleted',
            data: { "id": ResellerId, "actionId": ActionId },
            type: "POST",
            error: function (data) {
                window.location.href = "Home?err=1";
            }
        }
    });
}