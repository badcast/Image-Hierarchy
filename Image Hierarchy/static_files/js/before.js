$(document).ready(function() {
    // ui-----------------------------------

    document.getElementById("signOutSignInPage").style.display = "none";

    (async function() {

        document.getElementById("sSignUp").addEventListener("click", async function(e) {
            e.preventDefault();

            var username = "";
            var password = "";

            function go() {


            }

            function back() {

            }


        });

        // получаем токен
        document.getElementById("logIn").addEventListener("click", async function(e) {
            e.preventDefault();
            ev_login(false);
        });

        // условный выход - просто удаляем токен и меняем видимость блоков
        document.getElementById("logOut").addEventListener("click", e => {
            e.preventDefault();
            client.logout();

            document.getElementById("errorMessage").innerText = "";
            document.getElementById("dUserName").innerText = "";
            document.getElementById("signedForm").style.display = "none";
            document.getElementById("loginForm").style.display = "block";
            document.getElementById("signOutSignInPage").style.display = "block";
            setTimeout(() => {
                document.getElementById("signOutSignInPage").firstElementChild.style.display = "none";
                document.getElementById("signOutSignInPage").lastElementChild.style.display = "block";
            }, 500);

        });

        document.getElementById("sSignUp").addEventListener("click", e => {
            e.preventDefault();

            if (!$("__slicense").value) {
                $("_sErrorMessage").text("Is not accepted the license");
                return;
            }
            $("_sErrorMessage")
            client.register({
                    login: "lightmistercf",
                    password: "admin061099",
                    displayName: "Русский_демократ",
                    phoneNumber: "87021094814",
                    email: "lmecomposer@gmail.com"
                })
                .then(json => {

                }).catch(err => {

                });

        });

        async function onSigned() {
            let images = await client.getImages();
            let elem = $("#dImages")
            if (images.length == 0) {
                elem.text("У вас пока нет загруженных картинок");
            } else {

            }
        }

        function on_sign_OUT() {

        }

        async function ev_login(only = false) {
            async function go() {
                try {

                    $("#signOutSignInPage").css({ display: "none" });
                    $("#dUserName").text(await client.getDisplayName());
                    $("#dUserID").text(client.userId().toString());
                    $("#dUserPhoneNumber").text(await client.getPhoneNumber());
                    $("#dUserEmail").text(await client.getEmail());
                    $("#dUserVerifyStatus").text((await client.verifyUser()) ? "Yes" : "No");
                    $("#loginForm").css({ display: "none" });
                    $("#signedForm").css({ display: "block" });

                    onSigned();
                } catch (ex) {
                    back(ex.message);
                }
            }

            function back(message = undefined) {
                document.getElementById("signOutSignInPage").style.display = "block";
                document.getElementById("signOutSignInPage").firstElementChild.style.display = "none";
                document.getElementById("signOutSignInPage").lastElementChild.style.display = "block";

                let elem = $("#errorMessage");
                if (message != undefined) {
                    elem.css({ display: "block" });
                    elem.text(message);
                } else {
                    elem.css({ display: none });
                    elem.text("");
                }
            }

            if (!only) {
                setTimeout(() => {
                    client.login($("#emailLogin")[0].value, $("#passwordLogin")[0].value)
                        .then(json => {
                            if (json.result)
                                go();
                            else
                                back(json.message);

                        }).catch(e => {
                            back(e);
                        })
                }, 500);
                document.getElementById("signOutSignInPage").firstElementChild.style.display = "block";
                document.getElementById("signOutSignInPage").lastElementChild.style.display = "none";
            } else {
                await go();
            }
        }

        const accesToken = await client.getToken();
        if (accesToken != null) {
            ev_login(true);
        } else {
            document.getElementById("signOutSignInPage").style.display = "block";
        }
    })()
});

function fetch_form(pageIndex = 0) {
    switch (pageIndex) {
        case 0:
            //login form
            document.getElementById('loginForm').style.display = 'block';
            document.getElementById('registerForm').style.display = 'none';
            break;
        case 1:
            //register form
            document.getElementById('loginForm').style.display = 'none';
            document.getElementById('registerForm').style.display = 'block';
            break;
    }
}