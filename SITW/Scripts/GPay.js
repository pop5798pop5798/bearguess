document.addEventListener('DOMContentLoaded', () => {
    var canUseGooglePay = false
    TPDirect.setupSDK(13269, 'app_IuNqgANRFkfWv6x4C6u4Wr2pJPO4cQoJVBlpdv1laUHbsPlgMHQ1zlRHH01j', 'sandbox');

    var googlePaySetting = {
        googleMerchantId: "Come from google portal",
        tappayGoogleMerchantId: "Come from tappay portal",
        allowedCardAuthMethods: ["PAN_ONLY", "CRYPTOGRAM_3DS"],
        merchantName: "Funbet Test!",
        emailRequired: true, // optional
        shippingAddressRequired: true, // optional,
        billingAddressRequired: true, // optional
        billingAddressFormat: "MIN", // FULL, MIN

        allowPrepaidCards: true,
        allowedCountryCodes: ['TW'],

        phoneNumberRequired: true // optional
    };
    TPDirect.googlePay.setupGooglePay(googlePaySetting);

    var paymentRequest = {
        allowedNetworks: ["AMEX", "JCB", "MASTERCARD", "VISA"],
        price: "1", // optional
        currency: "TWD" // optional
    };
    /*TPDirect.googlePay.setupPaymentRequest(paymentRequest, function (err, result) {
        if (result.canUseGooglePay) {
            TPDirect.googlePay.setupGooglePayButton({
                el: "#container",
                color: "black",
                type: "long",
                getPrimeCallback: function (err, prime) {
                    console.log('paymentRequestApi.getPrime result', prime)
                    handlePayByPrime(prime);
                }
            });
        }
    });*/
    TPDirect.googlePay.setupPaymentRequest(paymentRequest, function (err, result) {
        if (result.canUseGooglePay) {
            canUseGooglePay = true;
        }
    });
    


});