import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import AuthPage from "./pages/AuthPage";
import UsersPage from "./pages/UsersPage";


export default function App() {
    return (
        <Routes>
            <Route path="/" element={<AuthPage/>}/>
            <Route path="/users" element={<UsersPage/>}/>
        </Routes>
    );
}
