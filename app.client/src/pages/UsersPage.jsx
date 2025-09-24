import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function UsersPage() {
    const [users, setUsers] = useState([]);
    const [page, setPage] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const pageSize = 20;
    const navigate = useNavigate();

    const token = localStorage.getItem("token");

    const fetchUsers = (currentPage) => {
        setLoading(true);
        fetch(`http://localhost:5298/api/Users?page=${currentPage}&pageSize=${pageSize}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
            .then((res) => res.json())
            .then((data) => {
                setUsers(data.users);
                setTotalCount(data.totalCount);
            })
            .catch((err) => console.error("Error fetching users:", err))
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        fetchUsers(page);
    }, [page]);

    const totalPages = Math.ceil(totalCount / pageSize);

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/");
    };

    return (
        <div className="p-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Список користувачів</h1>
                <button
                    onClick={handleLogout}
                    className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-800"
                >
                    Вихід
                </button>
            </div>

            {loading ? (
                <div className="text-center py-10 text-gray-600">Завантаження користувачів...</div>
            ) : users.length === 0 ? (
                <div className="text-center py-10 text-gray-600">Користувачів немає на цій сторінці</div>
            ) : (
                <>
                    <table className="w-full border border-gray-300">
                        <thead className="bg-gray-200">
                        <tr>
                            <th className="border p-2">Фото</th>
                            <th className="border p-2">Ім'я</th>
                            <th className="border p-2">Email</th>
                            <th className="border p-2">Спосіб входу</th>
                        </tr>
                        </thead>
                        <tbody>
                        {users.map((u) => (
                            <tr key={u.id}>
                                <td className="border p-2">
                                    <img
                                        src={u.image || "https://via.placeholder.com/50"}
                                        alt="avatar"
                                        className="w-12 h-12 rounded-full object-cover"
                                    />
                                </td>
                                <td className="border p-2">{u.fullName}</td>
                                <td className="border p-2">{u.email}</td>
                                <td className="border p-2 text-center">{u.loginType}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>

                    {/* Пагінація */}
                    <div className="mt-4 flex justify-center items-center gap-4">
                        <button
                            onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
                            disabled={page === 1}
                            className="px-4 py-2 bg-gray-300 rounded disabled:opacity-50"
                        >
                            ⬅ Попередня
                        </button>
                        <span>
                            Сторінка {page} з {totalPages}
                        </span>
                        <button
                            onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
                            disabled={page === totalPages}
                            className="px-4 py-2 bg-gray-300 rounded disabled:opacity-50"
                        >
                            Наступна ➡
                        </button>
                    </div>
                </>
            )}
        </div>
    );
}