export function paypalStyle(baseUrl) {

    var apiUrl = baseUrl + 'Paypal/';
    console.log(apiUrl);
    paypal.Buttons({
        createOrder() {

            return fetch(apiUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(response => {

                console.log(response);
                return response.json();
            })
            .then((order) => {
                console.log(order);
                return order.id;
            });
        },
        onApprove(data) {
            console.log('onApprove');
            console.log(data);
            // This function captures the funds from the transaction.
            return fetch(apiUrl + "Capture?orderId="+ data.orderID, {
                method: "GET"
                })
                .then((response) => response.json())
                .then((details) => {
                    console.log(details);
                });
        }
    }).render('#paypal-button-container');
}