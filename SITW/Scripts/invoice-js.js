function wait_invoice(ajaxurl) {
    $("#page_invoice").css("display", "block");
    $("#gomain").on('click', function () {
        $("#bearModal").modal('toggle');

    });

    $("#invoiceModels").on('change', function () {
        var iM = $("input[name=invoiceModel]:checked").val();

        if (iM == 1) {
            $("#page_invoice").css("display", "block");
            $("#allval").css("display", "none");
            $("#page_only").css("display", "none");
            $("#page_company").css("display", "none");
            $("#page_company").css("margin-bottom", "0");
            $("#allval").css("margin-bottom", "0");
        } else if (iM == 2) {
            $("#page_invoice").css("display", "none");
            $("#page_only").css("display", "block");
            $("#allval").css("display", "block");
            $("#page_company").css("display", "none");
            $("#page_company").css("margin-bottom", "0");
            $("#allval").css("margin-bottom", "20px");
            $("#mailname").text("收件人姓名");
            $("#Emailinv").css("display", "block");

        } else if (iM == 3) {
            $("#page_invoice").css("display", "none");
            $("#page_only").css("display", "none");
            $("#allval").css("display", "block");
            $("#page_company").css("display", "block");
            $("#page_company").css("margin-bottom", "20px");
            $("#allval").css("margin-bottom", "0");
            $("#mailname").text("買受人公司名稱");
            $("#Emailinv").css("display", "block");

        }

    });
    $("select[name='CarrierType']").on('change', function () {
        if ($(this).val() != 1) {
            $("#CarrierID").css("display", "block");
            $("#allval").css("display", "none");
            if ($(this).val() == "3J0002")
                $("input[name='CarrierID']").attr("placeholder", "請輸入開頭為'/'之大寫英數字");
            if ($(this).val() == "CQ0001")
                $("input[name='CarrierID']").attr("placeholder", "請輸入2碼大寫字母+14碼數字");
        }
        else {
            $("#CarrierID").css("display", "none");
            $("#allval").css("display", "block");
        }


    });



    //#CarrierID

    $("#endsumit").on('click', function () {
        // var data = $("#invoice").serialize();
        var lovekey = "";
        var data = [];
        var iM = $("input[name=invoiceModel]:checked").val();
        if ($("input[name='donation']:checked").val() == 1) {
            lovekey = $("select[name='LoveKey']").val();
        } else if ($("input[name='donation']:checked").val() == 2) {

            lovekey = $("input[name='LoveKey2']").val();
        }


        if (iM == 1) {
            data.push({ "LoveKey": lovekey, "CarrierType": "", "CarrierID": "", "Email": $("input[name='Email']").val(), "Name": "", "Address": "", "Buyer_id": "", "MerchantOrder": $("input[name='MerchantOrder']").val(), "City": "", "County": "", "Model": iM });

        } else if (iM == 2) {
            if ($("select[name='CarrierType']").val() == 1)
                data.push({ "LoveKey": "", "CarrierType": $("select[name='CarrierType']").val(), "CarrierID": "", "Email": $("input[name='Email']").val(), "Name": $("input[name='Name']").val(), "Address": $("input[name='Address']").val(), "Buyer_id": "", "MerchantOrder": $("input[name='MerchantOrder']").val(), "City": $("select[name='City']").val(), "County": $("select[name='County']").val(), "Model": iM });
            else
                data.push({ "LoveKey": "", "CarrierType": $("select[name='CarrierType']").val(), "CarrierID": $("input[name='CarrierID']").val(), "Email": $("input[name='Email']").val(), "Name": $("input[name='Name']").val(), "Address": $("input[name='Address']").val(), "Buyer_id": "", "MerchantOrder": $("input[name='MerchantOrder']").val(), "City": $("select[name='City']").val(), "County": $("select[name='County']").val(), "Model": iM });
        } else if (iM == 3) {
            data.push({ "LoveKey": "", "CarrierType": "", "CarrierID": "", "Email": $("input[name='Email']").val(), "Name": $("input[name='Name']").val(), "Address": $("input[name='Address']").val(), "Buyer_id": $("input[name='Buyer_id']").val(), "MerchantOrder": $("input[name='MerchantOrder']").val(), "City": $("select[name='City']").val(), "County": $("select[name='County']").val(), "Model": iM });
        }

        $.ajax({
            url: ajaxurl,
            contentType: "application/json",
            type: "POST",
            data: JSON.stringify(data),
            async: true,
            success: function (e) {
                if (e == "success") {
                    $("form[name='newebpay']").submit();
                    $("#bearModal").modal('toggle');
                } else if (e == "buyererror") {
                    Swal.fire("統一編號錯誤",
                        "請再確認統一編號是否正確",
                        "error");

                } else if (e == "lovekeyerror") {
                    Swal.fire("捐贈碼錯誤",
                        "請再確認捐贈碼是否正確",
                        "error");

                } else if (e == "codeerror") {
                    Swal.fire("載具錯誤",
                        "請再確認載具碼是否正確",
                        "error");

                } else {
                    Swal.fire("發票填寫錯誤",
                        "欄位填寫不完成",
                        "error");
                }


            }

        });


    });




    


}