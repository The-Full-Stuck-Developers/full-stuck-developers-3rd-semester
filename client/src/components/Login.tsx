import {useState} from "react";
import {useNavigate} from "react-router";
import {type SubmitHandler, useForm} from "react-hook-form";
import type {LoginRequest} from "@core/generated-client.ts";
import toast from "react-hot-toast";
import {useAuth} from "../hooks/auth.tsx";
import Logo from "../jerneif-logo.png";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPass, setShowPass] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const {login} = useAuth();
    const {
        register,
        handleSubmit,
        formState: {errors},
    } = useForm<LoginRequest>();

    const onSubmit: SubmitHandler<LoginRequest> = async (data) => {
        await toast.promise(login(data), {
            loading: "Checking credentials...",
            success: "Welcome back!",
            error: "Invalid email or password",
        });
        navigate("/");
    };

    /*async function handleLogin(e: React.FormEvent) {
        e.preventDefault();
        try {
            // Backend request will be added later
            console.log("Login with", email, password);
        } catch {
            setError("Invalid email or password");
        }
    }*/

    return (
        <div className="min-h-screen flex justify-center items-center bg-white px-6 relative">
            {/* BACK BUTTON - Top Left */}
            <button
                onClick={() => navigate("/")}
                className="absolute top-6 left-6 flex items-center gap-2 text-gray-700 hover:text-gray-900 font-medium transition"
            >
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-5 w-5"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                >
                    <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M10 19l-7-7m0 0l7-7m-7 7h18"
                    />
                </svg>
                Back to Home
            </button>
            <form
                onSubmit={handleSubmit(onSubmit)}
                className="w-full max-w-md flex flex-col gap-5"
            >
                {/* LOGO - Top Center */}
                <div className="flex justify-center mb-6">
                    <img
                        src={Logo}
                        alt="Jerne IF Logo"
                        className="w-32 h-32 object-contain"
                        onError={(e) => {
                            e.currentTarget.style.display = "none";
                        }}
                    />
                </div>
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

            </form>
        </div>
    );
}
