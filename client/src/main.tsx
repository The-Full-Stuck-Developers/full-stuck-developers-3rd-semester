import {createRoot} from 'react-dom/client'
import './index.css'
import App from "../src/components/App"
import {StrictMode} from "react";
import {BrowserRouter} from "react-router";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <App/>
        </BrowserRouter>
    </StrictMode>,
)
