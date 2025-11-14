import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "@components/Home.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools/styles.css'
import {Toaster} from "react-hot-toast";
import Auth from "@components/routes/auth/Auth.tsx";

function App() {
    return (
        <>
            <RouterProvider router={createBrowserRouter([
                {
                    path: '',
                    element: <Home/>,
                    children: [
                        {
                            path: '',
                            element: <Auth />,
                            index: true
                        },
                    //     here you can just make more components: i just have a a single auth page with a quick crud test
                        
                    ]
                }
            ])}/>
            <DevTools/>
            <Toaster
                position="top-center"
                reverseOrder={false}
            />
        </>
    )
}

export default App
