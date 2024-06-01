
const tempCourseKey = "TempCourse"
export function saveTempCourse(serializedCourse) {
    localStorage.setItem(tempCourseKey, serializedCourse);
}
export function loadTempCourse() {
    return localStorage.getItem(tempCourseKey);
}