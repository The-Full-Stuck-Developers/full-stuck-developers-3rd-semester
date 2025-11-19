import {Home} from "../components/Home.tsx";
import {Route, Routes} from "react-router-dom";
function App() {
    return (
        <>
            <Routes>
                <Route path="/" element={<Home />} />
            </Routes>
          <Home/>
        </>
    )
}

export default App
