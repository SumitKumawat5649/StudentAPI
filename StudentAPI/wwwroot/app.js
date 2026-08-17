var API_BASE_URL = "https://localhost:7266";

document.addEventListener("DOMContentLoaded", function () {
    var token = localStorage.getItem("token");

    // 1. लॉगिन लॉजिक
    var loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", function (e) {
            e.preventDefault();
            var loginData = {
                Username: document.getElementById("username").value,
                PasswordHash: document.getElementById("password").value
            };
            fetch(API_BASE_URL + "/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(loginData)
            })
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    localStorage.setItem("token", data.token);
                    window.location.href = "dashboard.html";
                })
                .catch(function () { alert("लॉगिन फेल या API बंद है!"); });
        });
    }

    // 2. मास्टर डैशबोर्ड लॉजिक (All-in-One सिंगल व्यू)
    var studentTableBody = document.getElementById("studentTableBody");
    if (studentTableBody) {
        if (!token) { window.location.href = "login.html"; return; }

        // कोर्सेस लिस्ट टैब का साधारण अलर्ट (यदि उपयोग करना चाहें)
        var coursesTabBtn = document.getElementById("coursesTabBtn");
        if (coursesTabBtn) {
            coursesTabBtn.addEventListener("click", function () {
                alert("कोर्सेस देखने के लिए आप अपनी पुरानी कोर्सेस API का उपयोग कर सकते हैं।");
            });
        }

        // सभी छात्रों का पूरा रिलेशनल डेटा एक साथ लोड करना
        function loadMasterDashboard() {
            fetch(API_BASE_URL + "/api/Students", {
                method: "GET",
                headers: { "Authorization": "Bearer " + token, "Content-Type": "application/json" }
            })
                .then(function (r) { return r.json(); })
                .then(function (students) {
                    studentTableBody.innerHTML = "";

                    // हर छात्र के लिए डेटा प्रोसेस करना
                    students.forEach(function (student) {
                        var id = student.id || student.Id;
                        var name = student.name || student.Name;
                        var desc = student.description || student.Description || '-';
                        var age = student.age || student.Age || '-';
                        var cId = student.courseId || student.CourseId;

                        var cName = (student.courseDetails || student.CourseDetails) ?
                            ((student.courseDetails || student.CourseDetails).courseName || (student.courseDetails || student.CourseDetails).CourseName) : (cId ? "ID: " + cId : '-');

                        // डिफ़ॉल्ट खाली मान (अगर कोई परीक्षा न हो)
                        var examName = "No Exam Planned";
                        var marksDisplay = "-";
                        var statusDisplay = "-";

                        // अगर छात्र का कोर्स वैलिड है, तो उसके मार्क्स और एग्जाम लाइव निकालना
                        if (cId) {
                            // पहले उसके कोर्स की एग्जाम आईडी ढूंढने के लिए एक छोटा इंटरनल फेच
                            fetch(API_BASE_URL + "/api/Exams/course/" + cId, {
                                method: "GET",
                                headers: { "Authorization": "Bearer " + token }
                            })
                                .then(function (res) { return res.json(); })
                                .then(function (exams) {
                                    if (exams.length > 0) {
                                        var firstExam = exams[0];
                                        examName = firstExam.examName || firstExam.ExamName;
                                        var totalMarks = firstExam.totalMarks || firstExam.TotalMarks;

                                        // अब उस एग्जाम आईडी से इस विशिष्ट छात्र के नंबर निकालना
                                        return fetch(API_BASE_URL + "/api/Exams/" + (firstExam.id || firstExam.Id) + "/results", {
                                            method: "GET",
                                            headers: { "Authorization": "Bearer " + token }
                                        })
                                            .then(function (res2) { return res2.json(); })
                                            .then(function (results) {
                                                // इस छात्र का रिकॉर्ड ढूंढें
                                                var myResult = results.find(function (r) { return r.studentId === id || r.StudentId === id; });
                                                if (myResult) {
                                                    var marksObtained = myResult.marksObtained || myResult.MarksObtained || 0;
                                                    var status = myResult.status || myResult.Status || '-';

                                                    marksDisplay = marksObtained + " / " + totalMarks;
                                                    statusDisplay = status;
                                                }
                                                updateRowInTable(id, name, desc, age, cName, examName, marksDisplay, statusDisplay);
                                            });
                                    } else {
                                        updateRowInTable(id, name, desc, age, cName, examName, marksDisplay, statusDisplay);
                                    }
                                })
                                .catch(function () {
                                    updateRowInTable(id, name, desc, age, cName, examName, marksDisplay, statusDisplay);
                                });
                        } else {
                            updateRowInTable(id, name, desc, age, cName, examName, marksDisplay, statusDisplay);
                        }
                    });
                });
        }

        // टेबल में रो को जोड़ने या अपडेट करने का हेल्पर फंक्शन
        function updateRowInTable(id, name, desc, age, cName, examName, marksDisplay, statusDisplay) {
            // चेक करें कि क्या इस छात्र की रो पहले से मौजूद है
            var existingRow = document.getElementById("row-" + id);

            var statusStyle = "font-weight:bold;";
            if (statusDisplay === "Pass") statusStyle += "color:#28a745;";
            else if (statusDisplay === "Fail") statusStyle += "color:#dc3545;";

            var rowHTML = "<td>" + id + "</td>" +
                "<td><strong>" + name + "</strong></td>" +
                "<td>" + desc + "</td>" +
                "<td>" + age + "</td>" +
                "<td>" + cName + "</td>" +
                "<td style='color:#007bff; font-weight:bold;'>" + examName + "</td>" +
                "<td style='font-weight:bold; color:#6f42c1;'>" + marksDisplay + "</td>" +
                "<td style='" + statusStyle + "'>" + statusDisplay + "</td>";

            if (existingRow) {
                existingRow.innerHTML = rowHTML;
            } else {
                var tr = document.createElement("tr");
                tr.id = "row-" + id;
                tr.innerHTML = rowHTML;
                studentTableBody.appendChild(tr);
            }
        }

        loadMasterDashboard();
    }

    // 3. लॉगआउट लॉजिक
    var logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", function () {
            localStorage.removeItem("token");
            window.location.href = "login.html";
        });
    }
});