$(document).ready(function () {
    //Partner Registration validation
    var validate = $("#jvalidate").validate({
        ignore: [],
        rules: {
            "CompanyName": {
                required: true,
                minlength: 2
            },
            "ResellerName": {
                required: true,
                minlength: 2
            },
            "MerchantName": {
                required: true
            },
            "POSName": {
                required: true
            },
            "CompanyEmail": {
                required: true,
                email: true
            },
            "ResellerEmail": {
                required: true,
                email: true
            },
            "MDR": {
                required: true,
                number: true
            },
            "User.FirstName": {
                required: true
            },
            "User.LastName": {
                required: true
            },
            "User.EmailAddress": {
                required: true,
                email: true
            },
            "User.Username": {
                required: true
            },
            "User.Password": {
                required: true,
                minlength: 8
            },
            "User.ConfirmPassword": {
                required: true,
                equalTo: "#User_Password"
            },
            "FirstName": {
                required: true
            },
            "LastName": {
                required: true
            },
            "Address": {
                required: true
            },
            "City": {
                required: true
            },
            "ZipCode": {
                required: true
            },
            "PrimaryContactNumber": {
                required: true
            },
            "EmailAddress": {
                required: true,
                email: true
            },
            "Username": {
                required: true
            },
            "Password": {
                required: true,
                minlength: 8
            },
            "ConfirmPassword": {
                required: true,
                equalTo: "#Password"
            },
            "User.PIN": {
                required: true,
                minlength: 4,
                maxlength: 4,
                number: true
            },
            "PIN": {
                required: true,
                minlength: 4,
                maxlength: 4,
                number: true
            }
        }
    });

    
    //Mids Registration validation
    var validate = $("#reg_form").validate({
        ignore: [],
        rules: {
            "MIDName": {
                required: true
            },
            "TC_Discount": {
                required: true
            },
            "TC_CardNotPresent": {
                required: true
            },
            "TC_Decline": {
                required: true
            },
            "TC_eCommerce": {
                required: true
            },
            "TC_PreAuth": {
                required: true
            },
            "TC_Capture": {
                required: true
            },
            "TC_Purchased": {
                required: true
            },
            "TC_Refund": {
                required: true
            },
            "TC_Void": {
                required: true
            },
            "TC_CashBack": {
                required: true
            },
            "TC_BalanceInquiry": {
                required: true
            }
        }
    });

    //Device Creation validation
    var validate = $("jvalidate-device").validate({
        ignore: [],
        rules: {
            "DeviceName": {
                required: true
            },
            "Manufacturer": {
                required: true
            },
            "Warranty": {
                required: true
            }
        }
    });


    //Users
    var validatejj = $("#jvalidate-user").validate({
        ignore: [],
        rules: {
            "CompanyName": {
                required: true,
                minlength: 2
            },
            "ResellerName": {
                required: true,
                minlength: 2
            },
            "MerchantName": {
                required: true
            },
            "POSName": {
                required: true
            },
            "CompanyEmail": {
                required: true,
                email: true
            },
            "ResellerEmail": {
                required: true,
                email: true
            },
            "MDR": {
                required: true,
                number: true
            },
            "EmailAddress": {
                required: true,
                email: true
            },
            "Password": {
                required: true,
                minlength: 8
                //regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$_@=|{}+!#<>])[a-zA-Z0-9$_@=|{}+!#<>]/
            },
            "ConfirmPassword": {
                required: true,
                equalTo: "#User_Password"
            },
            "FirstName": {
                required: true
            },
            "LastName": {
                required: true
            },
            "Address": {
                required: true
            },
            "City": {
                required: true
            },
            "PrimaryContactNumber": {
                required: true
            },
            "User.Username": {
                required: true
            },
            "User.EmailAddress": {
                required: true,
                email: true
            },
            "Username": {
                required: true
            },
            "User.FirstName": {
                required: true
            },
            "User.LastName": {
                required: true
            },
            "User.PIN": {
                required: true
            },
            "User.Password": {
                required: true,
                minlength: 8,
                //regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$_@=|{}+!#<>])[a-zA-Z0-9$_@=|{}+!#<>]/
            },
            "User.ConfirmPassword": {
                required: true,
                equalTo: "#User_Password"
            },
            "PIN": {
                required: true,
                minlength: 4,
                number: true
            },
        }
    });

    var validatejj = $("#requestForm").validate({
        ignore: [],
        rules: {
            "MerchantName": {
                required: true,
            },
            "FirstName": {
                required: true,
            },
            "LastName": {
                required: true
            },
            "Email Address": {
                required: true,
                email: true
            },
            "Address": {
                required: true
            },
            "City": {
                required: true
            },
            "ZipCode": {
                required: true
            },
            "MIDName": {
                required: true
            },
            "SecureHash": {
                required: true
            },
            "AccessCode": {
                required: true
            },
            "PrimaryContactNumber": {
                required: true
        },
    }
    });

    var validatejj = $("#formRequest").validate({
        ignore: [],
        rules: {
            "BranchName": {
                required: true,
            },
            "Address": {
                required: true,
            },
            "City": {
                required: true
            },
            "StateProvince": {
                required: true
            },
            "ZipCode": {
                required: true
            },
            "PrimaryContactNumber": {
                required: true
            },
            "Fax": {
                required: true
            },
            "MobileNumber": {
                required: true
            },
            "User.FirstName": {
                required: true
            },
            "AccessCode": {
                required: true
            },
            "User.LastName": {
                required: true
            },
            "User.Address": {
                required: true
            },
            "User.City": {
                required: true
            },
            "User.StateProvince": {
                required: true
            },
            "User.ZipCode": {
                required: true
            },
            "User.PrimaryContactNumber": {
                required: true
            },
            "User.EmailAddress": {
                required: true,
                email: true
            },
            "User.Username": {
                required: true
            },
            "User.Password": {
                required: true
            },
            "User.PIN": {
                required: true
            },
            "MIDName": {
                required: true
            },
        }
    });


    var validatejj = $("#formRequestUser").validate({
        ignore: [],
        rules: {
            "FirstName": {
                required: true,
            },
            "LastName": {
                required: true,
            },
            "Address": {
                required: true
            },
            "City": {
                required: true
            },
            "ZipCode": {
                required: true
            },
            "PrimaryContactNumber": {
                required: true
            },
            "EmailAddress": {
                required: true
            },
            "Username": {
                required: true
            },
            "Password": {
                required: true
            },
            "ConfirmPassword": {
                required: true,
                equalTo: "#Password"
            },
            "PIN": {
                required: true
            }
        }
    });

    //Payment validation
    var validate = $("#jvalidatePay").validate({
        ignore: [],
        rules: {
            "FirstName": {
                required: true,
                minlength: 2
            },
            "LastName": {
                required: true,
                minlength: 2
            },
            "Amount": {
                required: true,
                number: true
            },
            "CardNumber": {
                required: true,
                number: true
            },
            "Cvv": {
                required: true,
                number: true
            },
            "Email": {
                required: true,
                email: true
            },
            "Phone": {
                required: true,
                minlength: 4
            },
            "GoodsTitle": {
                required: true,
                minlength: 2
            },
            "City": {
                required: true,
                minlength: 2
            },
            "State": {
                required: true,
                minlength: 2
            },
            "Zip": {
                required: true,
                minlength: 2
            },
            "Address": {
                required: true,
                minlength: 2
            },
            "InvoiceNumber": {
                required: true,
                minlength: 2
            }
        }
    });

    //Refund validation
    var validate = $("#jvalidateRefund").validate({
        ignore: [],
        rules: {
            "TransactionNumber": {
                required: true,
                minlength: 1
            }
        }
    });
});