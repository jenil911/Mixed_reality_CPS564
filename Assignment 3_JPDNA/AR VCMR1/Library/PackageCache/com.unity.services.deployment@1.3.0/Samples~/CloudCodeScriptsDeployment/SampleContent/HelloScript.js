module.exports = async ({ params, context, logger }) => {
    name = params["name"];
    return "Hello " + name + "!";
};

module.exports.params = {
    "name": "String"
}
