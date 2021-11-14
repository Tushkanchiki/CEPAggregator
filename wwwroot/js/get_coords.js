let ceps = @Html.Raw(Json.Serialize(Model));
console.log(ceps);
async function get_coords() {
    all_coords = [];
    for (let cep in ceps) {
        const address = cep["address"]["city"]["name"] + ", " + cep["address"]["name"];
        console.log(address);
        res = await ymaps.geocode(address, { results: 1 })
        if (res.geoObjects.getLength() > 0) {
            if (res.geoObjects.get(0).hasOwnProperty("geometry")) {
                var firstGeoObject = res.geoObjects.get(0),
                    coords = firstGeoObject.geometry.getCoordinates();
                all_coords.push({
                    BankName: cep["bankName"],
                    CustomId: cep["customId"],
                    x: coords[0],
                    y: coords[1]
                });
            }
        }
    }
    console.log(JSON.stringify(all_coords));
}

ymaps.ready(get_coords);