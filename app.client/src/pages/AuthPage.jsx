import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useGoogleLogin } from "@react-oauth/google";

export default function AuthPage() {
    const navigate = useNavigate();

    const [loginEmail, setLoginEmail] = useState("");
    const [loginPassword, setLoginPassword] = useState("");

    const [regFirstName, setRegFirstName] = useState("");
    const [regLastName, setRegLastName] = useState("");
    const [regEmail, setRegEmail] = useState("");
    const [regPassword, setRegPassword] = useState("");
    const [regImageFile, setRegImageFile] = useState(null);

    // --- Логін через Email/Password ---
    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const res = await fetch("http://localhost:5298/api/Account/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email: loginEmail, password: loginPassword }),
            });
            if (!res.ok) {
                alert("Невірний логін або пароль");
                return;
            }
            const data = await res.json();
            localStorage.setItem("token", data.jwtToken);
            navigate("/users");
        } catch (err) {
            console.error(err);
            alert("Помилка входу");
        }
    };

    // --- Реєстрація ---
    const handleRegister = async (e) => {
        e.preventDefault();

        const formData = new FormData();
        formData.append("firstName", regFirstName);
        formData.append("lastName", regLastName);
        formData.append("email", regEmail);
        formData.append("password", regPassword);
        if (regImageFile) {
            formData.append("imageFile", regImageFile);
        }

        try {
            const res = await fetch("http://localhost:5298/api/Account/register", {
                method: "POST",
                body: formData
            });

            if (!res.ok) {
                const err = await res.text();
                alert("Помилка реєстрації: " + err);
                return;
            }

            alert("Реєстрація успішна!");
            setTimeout(() => navigate("/users"), 1000);
        } catch (err) {
            console.error(err);
            alert("Помилка при реєстрації");
        }
    };

    const loginByGoogle = useGoogleLogin({
        onSuccess: async (tokenResponse) => {
            // Використовуємо access_token від Google
            const accessToken = tokenResponse?.access_token;
            if (!accessToken) {
                alert("Не отримано accessToken від Google");
                return;
            }

            try {
                // Надсилаємо токен на бекенд
                const res = await fetch("http://localhost:5298/api/Account/google-login", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ token: accessToken })
                });

                if (!res.ok) {
                    const err = await res.text();
                    alert("Помилка Google логіну: " + err);
                    return;
                }

                const data = await res.json();
                localStorage.setItem("token", data.jwtToken);
                navigate("/users");

            } catch (err) {
                console.error(err);
                alert("Помилка при Google логіні");
            }
        },
    });

    return (
        <div className="flex h-screen items-center justify-center bg-gray-100">
            <div className="flex bg-white shadow-lg rounded-xl overflow-hidden w-[800px]">

                {/* --- Логін --- */}
                <div className="w-1/2 p-6">
                    <h2 className="text-2xl font-bold mb-4 text-center">Вхід</h2>
                    <form onSubmit={handleLogin}>
                        <input
                            type="email"
                            placeholder="Email"
                            name="email"
                            autoComplete={"username"}
                            value={loginEmail}
                            onChange={(e) => setLoginEmail(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <input
                            type="password"
                            placeholder="Пароль"
                            name="password"
                            autoComplete={"current-password"}
                            value={loginPassword}
                            onChange={(e) => setLoginPassword(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <button
                            type="submit"
                            className="bg-blue-600 text-white w-full py-2 rounded hover:bg-blue-800 mb-3"
                        >
                            Увійти
                        </button>

                        <button
                            type="button"
                            onClick={() => loginByGoogle()} className="bg-red-600 text-white px-4 py-2 rounded">
                            Sign in with Google 🚀
                        </button>
                    </form>
                </div>

                <div className="w-[2px] bg-gray-300"></div>

                {/* --- Реєстрація --- */}
                <div className="w-1/2 p-6">
                    <h2 className="text-2xl font-bold mb-4 text-center">Реєстрація</h2>
                    <form onSubmit={handleRegister}>
                        <input
                            type="text"
                            placeholder="Ім'я"
                            value={regFirstName}
                            onChange={(e) => setRegFirstName(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <input
                            type="text"
                            placeholder="Прізвище"
                            value={regLastName}
                            onChange={(e) => setRegLastName(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <input
                            type="email"
                            placeholder="Email"
                            value={regEmail}
                            onChange={(e) => setRegEmail(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <div className="mb-3">
                            <label className="block text-gray-700 mb-1">Фото профілю</label>

                            {regImageFile && (
                                <img
                                    src={URL.createObjectURL(regImageFile)}
                                    alt="preview"
                                    className="w-24 h-24 object-cover rounded-full mb-2 border border-gray-300"
                                />
                            )}

                            <label
                                htmlFor="regImageFile"
                                className="cursor-pointer inline-block bg-gray-200 hover:bg-gray-300 text-gray-800 py-2 px-4 rounded"
                            >
                                {regImageFile ? "Змінити фото" : "Вибрати фото"}
                            </label>
                            <input
                                id="regImageFile"
                                type="file"
                                accept="image/*"
                                className="hidden"
                                onChange={(e) => setRegImageFile(e.target.files[0])}
                            />
                        </div>
                        <input
                            type="password"
                            placeholder="Пароль"
                            value={regPassword}
                            onChange={(e) => setRegPassword(e.target.value)}
                            className="w-full border p-2 mb-3 rounded"
                        />
                        <button
                            type="submit"
                            className="bg-green-600 text-white w-full py-2 rounded hover:bg-green-800"
                        >
                            Зареєструватись
                        </button>
                    </form>
                </div>

            </div>
        </div>
    );
}
