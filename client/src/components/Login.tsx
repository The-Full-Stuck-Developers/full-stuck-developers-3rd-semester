import { useState } from "react";
import axios from "axios";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPass, setShowPass] = useState(false);
    const [error, setError] = useState("");

    async function handleLogin(e: React.FormEvent) {
        e.preventDefault();
        try {
            // Backend request will be added later
            console.log("Login with", email, password);
        } catch {
            setError("Invalid email or password");
        }
    }

    return (
        <div className="min-h-screen flex justify-center items-center bg-white px-6">
            <form
                onSubmit={handleLogin}
                className="w-full max-w-md flex flex-col gap-5"
            >
                {/* HEADER */}
                <h1 className="text-4xl font-bold">Log in</h1>


                {/* EMAIL */}
                <label className="font-medium">Email</label>
                <input
                    type="email"
                    placeholder="Enter email"
                    onChange={(e) => setEmail(e.target.value)}
                    className="border rounded-xl px-4 py-3 text-lg w-full focus:outline-verde"
                />

                {/* PASSWORD FIELD */}
                <label className="font-medium">Password</label>
                <div className="relative">
                    <input
                        type={showPass ? "text" : "password"}
                        placeholder="Enter password"
                        onChange={(e) => setPassword(e.target.value)}
                        className="border rounded-xl px-4 py-3 text-lg w-full focus:outline-verde"
                    />
                    <button
                        type="button"
                        className="absolute right-3 top-3 text-gray-500"
                        onClick={() => setShowPass((p) => !p)}
                    >
                        👁
                    </button>
                </div>

                {/* FORGOT PASS */}
                <a className="text-sm underline font-medium cursor-pointer">
                    Forgot password?
                </a>

                {/* ERROR MSG */}
                {error && <p className="text-red-500 text-sm">{error}</p>}

                {/* LOGIN BUTTON */}
                <button
                    type="submit"
                    className="bg-red-700 hover:bg-red-800 transition text-white py-3 rounded-xl text-lg font-semibold"
                >
                    Log in
                </button>

                {/* CREATE ACCOUNT */}
                <button
                    type="button"
                    className="border py-3 rounded-xl text-lg font-semibold hover:bg-gray-100 transition"
                >
                    Create account
                </button>
            </form>
        </div>
    );
}
