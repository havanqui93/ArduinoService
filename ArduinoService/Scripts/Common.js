
// Check value is null or empty , true : null, false : not null.
function IsNullOrEmpty(value) {
    if (value == '' || value == null || value == undefined)
        return true;
    return false;
}