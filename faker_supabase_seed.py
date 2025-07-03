import psycopg2
from faker import Faker
import uuid

# Configuración de conexión
conn = psycopg2.connect(
    host="aws-0-us-east-1.pooler.supabase.com",
    port=5432,
    database="postgres",
    user="postgres.gdmzrpstylhqurhxqocw",
    password="J@flores2006"
)
cur = conn.cursor()
fake = Faker()

# Insertar 10 cursos
course_ids = []
for _ in range(10):
    course_id = str(uuid.uuid4())
    title = fake.sentence(nb_words=3)
    description = fake.text(max_nb_chars=100)
    cur.execute(
        'INSERT INTO courses ("Id", "Title", "Description", "IsPublished") VALUES (%s, %s, %s, %s)',
        (course_id, title, description, False)
    )
    course_ids.append(course_id)

# Insertar 10 módulos (uno por curso)
module_ids = []
for course_id in course_ids:
    module_id = str(uuid.uuid4())
    title = fake.sentence(nb_words=2)
    cur.execute(
        'INSERT INTO modules ("Id", "Title", "CourseId") VALUES (%s, %s, %s)',
        (module_id, title, course_id)
    )
    module_ids.append(module_id)

# Insertar 10 lecciones (una por módulo)
for module_id in module_ids:
    lesson_id = str(uuid.uuid4())
    title = fake.sentence(nb_words=2)
    content = fake.text(max_nb_chars=200)
    cur.execute(
        'INSERT INTO lessons ("Id", "Title", "Content", "ModuleId") VALUES (%s, %s, %s, %s)',
        (lesson_id, title, content, module_id)
    )

# Insertar 10 instructores
instructor_ids = []
for _ in range(10):
    instructor_id = str(uuid.uuid4())
    name = fake.name()
    cur.execute(
        'INSERT INTO instructors ("Id", "Name") VALUES (%s, %s)',
        (instructor_id, name)
    )
    instructor_ids.append(instructor_id)

# Asignar instructores a cursos (uno por curso)
for course_id, instructor_id in zip(course_ids, instructor_ids):
    cur.execute(
        'INSERT INTO course_instructors ("CourseId", "InstructorId") VALUES (%s, %s)',
        (course_id, instructor_id)
    )

conn.commit()
cur.close()
conn.close()
print("¡Datos de ejemplo insertados correctamente!") 