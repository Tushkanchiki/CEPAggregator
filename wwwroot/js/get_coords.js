let ceps = @Html.Raw(Json.Serialize(Model));
console.log(ceps);

async function get_coords() {
    all_coords = [];
    promises = [];
    for (let cep of ceps) {
        const address = "Беларусь, " + cep.address.city.name + ", " + cep.address.name;
        console.log(address);
        promises.push(ymaps.geocode(address, { results: 1 }));
    }
    for (let i = 0; i < ceps.length; i++) {
        let res = await promises[i];
        if (res.geoObjects.getLength() > 0) {
            if (res.geoObjects.get(0).hasOwnProperty("geometry")) {
                var firstGeoObject = res.geoObjects.get(0),
                    coords = firstGeoObject.geometry.getCoordinates();
                all_coords.push({
                    BankName: ceps[i].bankName,
                    CustomId: ceps[i].customId,
                    x: coords[0],
                    y: coords[1]
                });
            }
        }
    }
    console.log(JSON.stringify(all_coords));
}

ymaps.ready(get_coords);